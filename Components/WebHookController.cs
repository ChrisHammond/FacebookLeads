/*
' Copyright (c) 2018 Christoc.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using DotNetNuke.Web.Api;
using Christoc.Modules.FacebookLeads.Model;
using Newtonsoft.Json;
using DotNetNuke.Entities.Modules;
using System.Web.Http;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;

namespace Christoc.Modules.FacebookLeads.Components
{

    //http://dnnsummit2018.me/desktopmodules/FacebookLeads/API/Webhooks/Post?hub.challenge=test

    public class WebhooksController : DnnApiController
    {
        #region Get Request
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage Post()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(HttpContext.Current.Request.QueryString["hub.challenge"])
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        }
        #endregion Get Request

        #region Post Request
        [AllowAnonymous]
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] JsonData data)
        //public async Task<HttpResponseMessage> Post([FromBody] String data)
        {
            try
            {
                var objEventLog = new EventLogController();
                objEventLog.AddLog("Lead Request", "Lead Received from Facebook + Data=" + JsonConvert.SerializeObject(data)
                    , PortalSettings, -1, EventLogController.EventLogType.ADMIN_ALERT);
                if (data.Entry.Count > 0)
                {
                    var entry = data.Entry[0];

                    if (entry.Changes.Count > 0)
                    {
                        var change = entry?.Changes[0];

                        if (change == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);


                        //Get the token from the module settings.
                        //TODO: check if these tokens are 24 hour or not.
                        //TODO: these tokens are currently 1 or 2 hours, look at using long lived tokens https://developers.facebook.com/docs/facebook-login/access-tokens/expiration-and-extension 

                        var mc = new ModuleController();                        
                        var mi = mc.GetModuleByDefinition(0, "FacebookLeads");                        
                        string token = mi.ModuleSettings["accessToken"].ToString();
                       
                        //https://graph.facebook.com/v2.11/<LEAD_ID>
                        var leadUrl = $"https://graph.facebook.com/v2.11/{change.Value.LeadGenId}?access_token={token}";
                        var formUrl = $"https://graph.facebook.com/v2.11/{change.Value.FormId}?access_token={token}";

                        using (var httpClientLead = new HttpClient())
                        {
                            var response = await httpClientLead.GetStringAsync(formUrl);
                            if (!string.IsNullOrEmpty(response))
                            {
                                var jsonObjLead = JsonConvert.DeserializeObject<LeadFormData>(response);

                                try
                                {
                                    //If response is valid get the field data
                                    using (var httpClientFields = new HttpClient())
                                    {
                                        var responseFields = await httpClientFields.GetStringAsync(leadUrl);
                                        if (!string.IsNullOrEmpty(responseFields))
                                        {
                                            var jsonObjFields = JsonConvert.DeserializeObject<LeadData>(responseFields);
                                            //jsonObjFields.FieldData contains the field value

                                            var l = new Lead();                                            

                                            foreach (var v in jsonObjFields.FieldData)
                                            {
                                                if (v.Name == "phone_number")
                                                    l.CellPhone = v.Values.FirstOrDefault();
                                                if (v.Name == "first_name")
                                                    l.FirstName = v.Values.FirstOrDefault();
                                                if (v.Name == "last_name")
                                                    l.LastName = v.Values.FirstOrDefault();
                                                if (v.Name == "email")
                                                    l.Email = v.Values.FirstOrDefault();
                                            }

                                            CreateUser(l.Email, l.FirstName, l.LastName, l.CellPhone);

                                            var objEventLog2 = new EventLogController();
                                            objEventLog2.AddLog("Inside Lead Creation", "Lead Created from Facebook + CRUserId=" + " Name=" + l.FirstName + " LastName=" + l.LastName
                                                , PortalSettings, -1, EventLogController.EventLogType.ADMIN_ALERT);

                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    var objEventLog2 = new EventLogController();
                                    objEventLog2.AddLog("Lead Creation Failed", "Lead Creation Failed from Facebook + Ex=" + ex.StackTrace.ToString() + "Lead URL" + leadUrl
                                                , PortalSettings, -1, EventLogController.EventLogType.ADMIN_ALERT);
                                }
                            }
                        }
                        return new HttpResponseMessage(HttpStatusCode.OK);

                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                else return new HttpResponseMessage(HttpStatusCode.BadRequest);
               

            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error-->{ex.Message}");
                Trace.WriteLine($"StackTrace-->{ex.StackTrace}");

                var objEventLog2 = new EventLogController();
                objEventLog2.AddLog("Lead Creation Failed", "Lead Creation Failed from Facebook + Ex=" + ex.StackTrace.ToString()
                            , PortalSettings, -1, EventLogController.EventLogType.ADMIN_ALERT);

                return new HttpResponseMessage(HttpStatusCode.BadGateway);
            }
        }

        #endregion Post Request


        private void CreateUser(string email, string firstname, string lastname, string phoneNumber)
        {

            var mp = new AspNetMembershipProvider();

            var userInfo = new UserInfo();
            userInfo.Username = email;
            //set auto generated password
            userInfo.Membership.Password = mp.GeneratePassword();
            userInfo.Email = email;
            userInfo.FirstName = firstname;
            userInfo.LastName = lastname;
            userInfo.Username = email;
            userInfo.DisplayName = firstname + ' ' + lastname;
            userInfo.PortalID = 0; //TODO: hard coded for PortalID            


            var createStatus = mp.CreateUser(ref userInfo);
            if (createStatus == UserCreateStatus.InvalidPassword)
                throw new Exception("Invalid password");
            
            //save back to table with IsVerified set
            if (createStatus == UserCreateStatus.Success)
            {
                var roleName = "Registered Users";

                bool bc = false;
                var rc = new RoleController();

                RoleInfo newRole = rc.GetRoleByName(userInfo.PortalID, roleName);

                if (newRole != null && userInfo != null)
                {
                    bc = userInfo.IsInRole(roleName);
                    rc.AddUserRole(userInfo.PortalID, userInfo.UserID, newRole.RoleID, DateTime.MinValue, DateTime.MaxValue);
                    userInfo = UserController.GetUserById(userInfo.PortalID, userInfo.UserID);
                    bc = userInfo.IsInRole(roleName);
                }
            }
        }
    }
}

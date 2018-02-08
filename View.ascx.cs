/*
' Copyright (c) 2018  Christoc.com
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
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;

namespace Christoc.Modules.FacebookLeads
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// 
    /// Setup of Facebook App
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : FacebookLeadsModuleBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //https://developers.facebook.com/docs/marketing-api/guides/lead-ads/quickstart/webhooks-integration
                if (!Page.IsPostBack)
                {
                    AlreadySetup.Visible = false;
                    if (Settings.Contains("AppId"))
                    {
                        AppInfo.Visible = true; NoAppInfo.Visible = false;
                        if (Settings.Contains("accessToken"))
                        {
                            AppInfo.Visible = false;
                            AlreadySetup.Visible = true;
                            var accessToken = Settings["accessToken"].ToString();
                            hfAuthToken.Value = accessToken;

                        }

                        else
                        {
                            AlreadySetup.Visible = false;
                        }
                    }
                    //AppId not configured yet, we need to display a message
                    else
                    {
                        AppInfo.Visible = false; NoAppInfo.Visible = true;
                    }
                }

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }


        protected void lbAuth_Click(object sender, EventArgs e)
        {
            var accessToken = hfAuthToken.Value;
            var mc = new ModuleController();
            mc.UpdateModuleSetting(ModuleId, "accessToken", accessToken);
        }

       
    }

 
}
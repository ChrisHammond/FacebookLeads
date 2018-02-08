using System.Web;

namespace Christoc.Modules.FacebookLeads
{

    public class FacebookLogin : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            var accessToken = context.Request["accessToken"];
            context.Session["AccessToken"] = accessToken;

            context.Response.Redirect("/"); //TODO: figure out where to send them
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}

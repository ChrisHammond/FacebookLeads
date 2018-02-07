using DotNetNuke.Web.Api;

namespace Christoc.Modules.FacebookLeads.Components
{
    public class FacebookLeadsRouteMapper : IServiceRouteMapper
    {

        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("FacebookLeads", "Default", "{controller}/{action}",
                                     new[] { "Christoc.Modules.FacebookLeads.Components" });

            //mapRouteManager.MapHttpRoute("MyServices", "default", "{controller}/{action}", new {"MyServices"});
        }
    }
}
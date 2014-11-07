using System.Web.Http;
using Owin;

namespace Galaxy.Agent
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { controller = "root", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "controlleraction",
                routeTemplate: "{controller}/{action}"
            );

            appBuilder.UseWebApi(config);
        } 
    }
}
using System.Web.Http;
using Owin;

namespace Codestellation.Galaxy.WebEnd.Bootstrap
{
    internal static class WebApiBootstrap
    {
        public static void UseWebApi(this IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { controller = "root", id = RouteParameter.Optional }
                );
            config.Routes.MapHttpRoute(
                name: "controlleraction",
                routeTemplate: "api/{controller}/{action}"
                );

            appBuilder.UseWebApi(config);
        }
    }
}
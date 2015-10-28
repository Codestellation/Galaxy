using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Codestellation.Galaxy.WebEnd.Api;
using Codestellation.Galaxy.WebEnd.Infrastructure;

namespace Codestellation.Galaxy.Boostrapping
{
    public class WebApiInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes
                    .FromAssembly(Assembly.GetExecutingAssembly())
                    .BasedOn<ApiController>()
                    .LifestyleTransient(),
                Component
                    .For<HttpConfiguration>()
                    .Instance(GetHttpConfiguration(container))
                );
        }

        private HttpConfiguration GetHttpConfiguration(IWindsorContainer container)
        {
            var config = new HttpConfiguration();

            // this order doesn't matter
            ConfigureIoC(config, container);
            ConfigureRouting(config);
            return config;
        }

        private void ConfigureIoC(HttpConfiguration config, IWindsorContainer container)
        {
            config.Services.Replace(typeof(IHttpControllerActivator), new WindsorControllerActivator(container));
            config.Services.Replace(typeof(IHttpActionInvoker), new SingleThreadActionInvoker());
        }

        private void ConfigureRouting(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { controller = "root", id = RouteParameter.Optional }
                );
            config.Routes.MapHttpRoute(
                name: "controlleraction",
                routeTemplate: "api/{controller}/{action}"
                );
        }
    }
}
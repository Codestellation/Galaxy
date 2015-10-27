using System.Web.Http;
using Castle.Windsor;
using Nancy.Owin;
using Owin;

namespace Codestellation.Galaxy.WebEnd.Bootstrap
{
    public static class OwinStartup
    {
        public static void Configure(IAppBuilder app, IWindsorContainer container)
        {
            app.UseWindowsAuthentication();
            app.ServeEmbeddedFiles();
            app.MapSignalR();
            app.UseWebApi(container.Resolve<HttpConfiguration>());

            var options = new NancyOptions { Bootstrapper = new NancyBootstrapper(container) };
            app.UseNancy(options);
        }
    }
}
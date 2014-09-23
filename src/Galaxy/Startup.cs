using System.Net;
using Owin;

namespace Codestellation.Galaxy
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            UseWindowsAuthentication(app);

            // TODO: May throw such exception (at least on my windows 8.1 pro). Investigation needed.
            // The Nancy self host was unable to start, as no namespace reservation existed for the provided url(s).
            // Please either enable UrlReservations.CreateAutomatically on the HostConfiguration provided to 
            // the NancyHost, or create the reservations manually with the (elevated) command(s):
            // netsh http add urlacl url=http://+/ user=Everyone
            // On russian machines this command could look like this:
            // netsh http add urlacl url=http://+:80/ user=Все
            app.UseNancy();
        }
        private void UseWindowsAuthentication(IAppBuilder app)
        {
            var listener = (HttpListener)app.Properties["System.Net.HttpListener"];
            listener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;
        }

    }
}

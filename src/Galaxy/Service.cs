using System;
using System.Configuration;
using Nancy.Hosting.Self;

namespace Codestellation.Galaxy
{
    public class Service
    {
        private readonly NancyHost _nancyHost;

        public unsafe Service()
        {
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
            var uriString = string.Format("http://localhost:{0}", port);
            _nancyHost = new NancyHost(new Uri(uriString));
        }

        public void Start()
        {
            //TODO: May throw such exception (at least on my windows 8.1 pro). Investigation needed.

            //The Nancy self host was unable to start, as no namespace reservation existed for the provided url(s).
            //Please either enable UrlReservations.CreateAutomatically on the HostConfiguration provided to 
            //the NancyHost, or create the reservations manually with the (elevated) command(s):
            //netsh http add urlacl url=http://+/ user=Everyone
            //On russian machines this command could look like this:
            //netsh http add urlacl url=http://+:80/ user=Все

            _nancyHost.Start();
        }

        public void Stop()
        {
            _nancyHost.Stop();
            _nancyHost.Dispose();
        }
    }
}
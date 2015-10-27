using System;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Codestellation.Galaxy.Configuration;
using Codestellation.Galaxy.WebEnd.Bootstrap;
using Microsoft.Owin.Hosting;

namespace Codestellation.Galaxy
{
    public class Service
    {
        private IDisposable _owinHost;
        private readonly string _uriString;
        private WindsorContainer _container;

        public Service(ServiceConfig configuration)
        {
            _uriString = string.Format("http://*:{0}", configuration.WebPort);
        }

        public void Start()
        {
            _container = new WindsorContainer();
            _container.Install(FromAssembly.This());

            //TODO: May throw such exception (at least on my windows 8.1 pro). Investigation needed.

            //The Nancy self host was unable to start, as no namespace reservation existed for the provided url(s).
            //Please either enable UrlReservations.CreateAutomatically on the HostConfiguration provided to
            //the NancyHost, or create the reservations manually with the (elevated) command(s):
            //netsh http add urlacl url=http://+/ user=Everyone
            //On russian machines this command could look like this:
            //netsh http add urlacl url=http://+:80/ user=Все

            //more information http://msdn.microsoft.com/en-us/library/ms733768.aspx and https://github.com/NancyFx/Nancy/wiki/Self-Hosting-Nancy

            _owinHost = WebApp.Start(_uriString, builder => OwinStartup.Configure(builder, _container));
        }

        public void Stop()
        {
            if (_owinHost != null)
            {
                _owinHost.Dispose();
            }
            _container.Dispose();
        }
    }
}
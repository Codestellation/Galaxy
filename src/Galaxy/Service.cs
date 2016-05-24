using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Codestellation.Galaxy.Configuration;
using Codestellation.Galaxy.Host;
using Codestellation.Galaxy.Host.ConfigManagement;
using Codestellation.Galaxy.WebEnd.Bootstrap;
using Microsoft.Owin.Hosting;

namespace Codestellation.Galaxy
{
    public class Service : IService, IConfigAware<ServiceConfig>
    {
        private IDisposable _owinHost;
        private string _uriString;
        private WindsorContainer _container;

        public HostConfig HostConfig { get; set; }

        public ValidationResult Accept(ServiceConfig config)
        {
            var result = new ValidationResult();
            if (config.WebPort == 0)
            {
                var error = new ValidationError(nameof(ServiceConfig.WebPort), $"Should be between 1 and {ushort.MaxValue}");
                result.AddError(error);
                return result;
            }
            _uriString = $"http://*:{config.WebPort}";
            return result;
        }

        public void Start()
        {
            _container = new WindsorContainer();
            _container.Install(FromAssembly.This());
            _container.Register(
                Component
                    .For<HostConfig>()
                    .Instance(HostConfig));

            _owinHost = WebApp.Start(_uriString, builder => OwinStartup.Configure(builder, _container));
        }

        public void Stop()
        {
            _owinHost?.Dispose();
            _container?.Dispose();
        }
    }
}
using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Codestellation.Galaxy.WebEnd.SignalR;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.Boostrapping
{
    public class SignalRInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            GlobalHost.DependencyResolver = new WindsorDependencyResolver(container);
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();

            var serializer = JsonSerializer.Create(settings);

            container.Register(
                Classes
                    .FromThisAssembly()
                    .Where(IsSignalRComponent)
                    .WithServiceSelf()
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton(),
                Component
                    .For<JsonSerializer>()
                    .Instance(serializer)
                );
        }

        private bool IsSignalRComponent(Type candidate)
        {
            return !candidate.IsAbstract && candidate.Namespace.StartsWith(typeof(WindsorDependencyResolver).Namespace, StringComparison.Ordinal);
        }
    }
}
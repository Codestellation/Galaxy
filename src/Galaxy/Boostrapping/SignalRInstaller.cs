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
            var settings = new JsonSerializerSettings { ContractResolver = new SignalRContractResolver() };

            var serializer = JsonSerializer.Create(settings);
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);

            container.Register(
                Classes
                    .FromThisAssembly()
                    .Where(IsSignalRComponent)
                    .WithServiceSelf()
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton()
                );
        }

        private bool IsSignalRComponent(Type candidate)
        {
            return !candidate.IsAbstract && candidate.Namespace.StartsWith(typeof(HubFactory).Namespace, StringComparison.Ordinal);
        }
    }
}
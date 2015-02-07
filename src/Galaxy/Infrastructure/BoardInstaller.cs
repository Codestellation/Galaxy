using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.Infrastructure
{
    public class BoardInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes
                    .FromThisAssembly()
                    .Where(type => type.Name.EndsWith("Board", StringComparison.Ordinal))
                    .WithServiceSelf()
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton()
                );

            container.Register(
                Component
                    .For<DeploymentTaskProcessor>()
                    .LifestyleSingleton());
        }
    }
}
using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager;

namespace Codestellation.Galaxy.Boostrapping
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
                    .LifestyleSingleton(),
                Component
                    .For<Repository>()
                    .LifestyleSingleton(),

                Component
                    .For<TaskBuilder>()
                    .LifestyleTransient(),
                Component
                    .For<OperationBuilder>()
                    .LifestyleTransient(),
                Component
                    .For<DeploymentTaskProcessor>()
                    .LifestyleSingleton());
        }
    }
}
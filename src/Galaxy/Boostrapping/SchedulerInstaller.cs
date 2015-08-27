using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Codestellation.Pulsar;
using Codestellation.Pulsar.Schedulers;

namespace Codestellation.Galaxy.Boostrapping
{
    public class SchedulerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component
                    .For<IScheduler, ISchedulerController>()
                    .ImplementedBy<PulsarScheduler>()
                    .LifestyleSingleton()
                );
        }
    }
}
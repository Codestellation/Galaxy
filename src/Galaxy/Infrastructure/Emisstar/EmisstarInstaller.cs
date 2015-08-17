using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Codestellation.Emisstar;
using Codestellation.Emisstar.CastleWindsor.Facility;

namespace Codestellation.Galaxy.Infrastructure.Emisstar
{
    namespace Finam.VentureFx.Core.Bootstrap
    {
        public class EmisstarInstaller : IWindsorInstaller
        {
            public void Install(IWindsorContainer container, IConfigurationStore store)
            {
                container.AddFacility<EmisstarFacility>(x => { x.RegisterRuleBaseDispatcher<MultiThreadDispatcher>(); });

                container.Register(Classes
                    .FromThisAssembly()
                    .IncludeNonPublicTypes()
                    .Where(IsPureHandler)
                    .WithServiceAllHandlers()
                    .Configure(x => x.IsFallback())
                    .LifestyleTransient());
            }

            private bool IsPureHandler(Type type)
            {
                var hanldersCount = type.ImplementationOfGenericInterface(typeof(IHandler<>)).Length;

                var interfaceCount = type.GetInterfaces().Length;

                return hanldersCount > 0 && hanldersCount == interfaceCount;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Codestellation.Emisstar;
using Codestellation.Emisstar.CastleWindsor.Facility;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.Infrastructure.Emisstar;
using MediatR;

namespace Codestellation.Galaxy.Boostrapping
{
    namespace Finam.VentureFx.Core.Bootstrap
    {
        public class EmisstarInstaller : IWindsorInstaller
        {
            public void Install(IWindsorContainer container, IConfigurationStore store)
            {
                container.AddFacility<EmisstarFacility>(x => { x.RegisterRuleBaseDispatcher<MultiThreadDispatcher>(); });

                container.Register(
                    Classes
                        .FromThisAssembly()
                        .IncludeNonPublicTypes()
                        .Where(IsPureHandler)
                        .WithServiceAllHandlers()
                        .Configure(x => x.IsFallback())
                        .LifestyleTransient());

                InstallMediator(container);
            }

            private void InstallMediator(IWindsorContainer container)
            {
                container.Register(
                    Component
                        .For<IMediator>()
                        .ImplementedBy<Mediator>()
                        .LifestyleSingleton(),
                    Component
                        .For<SingleInstanceFactory>()
                        .UsingFactoryMethod<SingleInstanceFactory>(CreateInstance)
                        .LifestyleSingleton(),
                    Component
                        .For<MultiInstanceFactory>()
                        .UsingFactoryMethod<MultiInstanceFactory>(k => t => (IEnumerable<object>)k.ResolveAll(t))
                        .LifestyleSingleton(),
                    Classes
                        .FromThisAssembly()
                        .Where(IsSynchronousHandler)
                        .WithServiceAllInterfaces()
                        .LifestyleTransient(),
                    //Register custom async handlers here
                    Component
                        .For(typeof(IAsyncRequestHandler<,>))
                        .ImplementedBy(typeof(SynchronizedHandler<,>))
                        .IsFallback()
                        .LifestyleSingleton(),
                    Component
                        .For(typeof(IAsyncRequestHandler<>))
                        .ImplementedBy(typeof(SynchronizedHandler<>))
                        .IsFallback()
                        .LifestyleSingleton());
            }

            private static SingleInstanceFactory CreateInstance(IKernel kernel)
            {
                return t =>
                {
                    Type definition = t.GetGenericTypeDefinition();
                    if (definition == typeof(IRequestHandler<>) || definition == typeof(IRequestHandler<,>))
                    {
                        return null;
                    }

                    return kernel.Resolve(t);
                };
            }

            private bool IsSynchronousHandler(Type obj)
            {
                bool IsHandler(Type iFace)
                {
                    var definition = iFace.GetGenericTypeDefinition();

                    return definition == typeof(IRequestHandler<,>) || definition == typeof(IRequestHandler<>);
                }

                return obj.GetInterfaces().Any(iface => iface.IsGenericType && IsHandler(iface));
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
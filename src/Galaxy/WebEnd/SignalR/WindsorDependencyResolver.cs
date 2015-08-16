using System;
using System.Collections.Generic;
using Castle.Windsor;
using Microsoft.AspNet.SignalR;

namespace Codestellation.Galaxy.WebEnd.SignalR
{
    public class WindsorDependencyResolver : DefaultDependencyResolver
    {
        private readonly IWindsorContainer _container;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            _container = container;
        }

        public override object GetService(Type serviceType)
        {
            return _container.Kernel.HasComponent(serviceType) ? _container.Resolve(serviceType) : base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.Kernel.HasComponent(serviceType) ? (IEnumerable<object>)_container.ResolveAll(serviceType) : base.GetServices(serviceType);
        }
    }
}
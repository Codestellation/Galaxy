using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Castle.Windsor;

namespace Codestellation.Galaxy.WebEnd.Infrastructure
{
    public class WindsorControllerActivator : IHttpControllerActivator
    {
        private readonly IWindsorContainer _container;

        public WindsorControllerActivator(IWindsorContainer container)
        {
            _container = container;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = (IHttpController)_container.Resolve(controllerType);
            var releaser = new WindsorReleaser(_container, controller);
            request.RegisterForDispose(releaser);
            return controller;
        }

        private class WindsorReleaser : IDisposable
        {
            private readonly IWindsorContainer _container;
            private readonly object _controller;

            public WindsorReleaser(IWindsorContainer container, object controller)
            {
                _container = container;
                _controller = controller;
            }

            public void Dispose()
            {
                _container.Release(_controller);
            }
        }
    }
}
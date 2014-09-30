using System;
using System.Linq;
using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public abstract class WinServiceOperation : IOperation
    {
        protected readonly string ServiceName;
        private readonly ServiceController _controller;

        public WinServiceOperation(string serviceName)
        {
            ServiceName = serviceName;
            ServiceController[] services = ServiceController.GetServices();
            _controller = services.SingleOrDefault(item => item.ServiceName == ServiceName);
        }

        protected bool IsServiceExists()
        {
            return _controller != null;
        }

        public bool SkipIfNotFound { get; set; }

        protected void Execute(Action<ServiceController> controllerAction)
        {
            if (_controller == null)
            {
                var message = string.Format("Service '{0}' not found", ServiceName);

                if (SkipIfNotFound)
                {
                    return;
                }
                throw new InvalidOperationException(message);
            }
            controllerAction(_controller);
        }

        public abstract void Execute(DeploymentTaskContext context);
    }
}

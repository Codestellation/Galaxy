using System;
using System.ServiceProcess;
using NLog;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public enum WinErrors
    {
        FileNotFound = 0x00000002
    }

    public abstract class WinServiceOperation : IOperation
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly string ServiceName;
        private readonly ServiceController _controller;

        public WinServiceOperation(string serviceName)
        {
            ServiceName = serviceName;
            ServiceController[] candidates = ServiceController.GetServices();
            Logger.Debug("Looking for service '{0}'", serviceName);
            foreach (ServiceController candidate in candidates)
            {
                var found = candidate.ServiceName.Equals(serviceName, StringComparison.Ordinal);
                Logger.Debug("Checking '{0}' is {1}", candidate.ServiceName, found);
                if (found)
                {
                    Logger.Debug("Service found");
                    _controller = candidate;
                    break;
                }
            }
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
                var message = $"Service '{ServiceName}' not found";

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
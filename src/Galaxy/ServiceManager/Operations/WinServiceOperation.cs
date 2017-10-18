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

        protected DeploymentTaskContext Context { get; private set; }
        protected string ServiceName { get; private set; }
        private ServiceController _controller;

        public void Execute(DeploymentTaskContext context)
        {
            Context = context;
            ServiceName = context.ServiceName;
            ServiceController[] candidates = ServiceController.GetServices();
            Logger.Debug("Looking for service '{0}'", ServiceName);
            foreach (ServiceController candidate in candidates)
            {
                var found = candidate.ServiceName.Equals(ServiceName, StringComparison.Ordinal);
                Logger.Debug("Checking '{0}' is {1}", candidate.ServiceName, found);
                if (found)
                {
                    Logger.Debug("Service found");
                    _controller = candidate;
                    break;
                }
            }

            ExecuteInternal(context);
        }

        protected abstract void ExecuteInternal(DeploymentTaskContext context);

        protected bool IsServiceExists()
        {
            return _controller != null;
        }

        protected void Execute(Action<ServiceController> controllerAction)
        {
            if (_controller == null)
            {
                var message = $"Service '{ServiceName}' not found. Action skipped.";
                Context.BuildLog.WriteAsync(message);
                return;
            }
            controllerAction(_controller);
        }
    }
}
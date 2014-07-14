using Codestellation.Galaxy.Domain;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public abstract class WinServiceOperation : OperationBase
    {
        protected readonly string _serviceTargetPath;

        public WinServiceOperation(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {
            _serviceTargetPath = Path.Combine(_targetPath, _deployment.DisplayName);
        }

        protected bool IsServiceExists(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            return services.Any(item => item.ServiceName == serviceName);
        }
    }
}

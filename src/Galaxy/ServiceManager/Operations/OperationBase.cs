using System.IO;
using System.Text;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public abstract class OperationBase : IOperation
    {
        protected const string ServiceHostFileName = "Codestellation.Galaxy.Host.exe";

        protected readonly Deployment Deployment;
        protected readonly NugetFeed Feed;
        protected readonly string BasePath;
        protected readonly string ServiceFolder;

        public OperationBase(string basePath, Deployment deployment, NugetFeed feed)
        {
            BasePath = basePath;
            Deployment = deployment;
            Feed = feed;
            ServiceFolder = Path.Combine(BasePath, Deployment.DisplayName);
        }

        public abstract void Execute(StringBuilder buildLog);
    }
}
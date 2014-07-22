using System.IO;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public abstract class OperationBase
    {
        protected const string ServiceHostFileName = "Codestellation.Galaxy.Host.exe";

        protected readonly Deployment _deployment;
        protected readonly NugetFeed _feed;
        protected readonly string BasePath;
        protected readonly string ServiceFolder;

        public OperationBase(string basePath, Deployment deployment, NugetFeed feed)
        {
            BasePath = basePath;
            _deployment = deployment;
            _feed = feed;
            ServiceFolder = Path.Combine(BasePath, Deployment.DisplayName);
        }

        public NugetFeed Feed
        {
            get { return _feed; }
        }

        public Deployment Deployment
        {
            get { return _deployment; }
        }

        public abstract void Execute();
    }
}
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public abstract class OperationBase
    {
        protected const string ServiceHostFileName = "Codestellation.Galaxy.Host.exe";

        protected readonly Deployment _deployment;
        protected readonly NugetFeed _feed;
        protected readonly string _targetPath;

        public OperationBase(string targetPath, Deployment deployment, NugetFeed feed)
        {
            _targetPath = targetPath;
            _deployment = deployment;
            _feed = feed;
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
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{


    public abstract class OperationBase
    {
        protected const string serviceHostFileName = "Codestellation.Galaxy.Host.exe";

        OperationResult _result;
        string _details;
        readonly protected string _targetPath;
        readonly protected NugetFeed _feed;
        readonly protected Deployment _deployment;

        public NugetFeed Feed
        {
            get { return _feed; }
        }

        public Deployment Deployment
        {
            get { return _deployment; }
        }

        public OperationResult Result
        {
            get { return _result; }
        }

        public OperationBase(string targetPath, Deployment deployment, NugetFeed feed)
        {
            _targetPath = targetPath;
            _deployment = deployment;
            _feed = feed;
        }

        protected void StoreResult<T>(T serviceOperation, ResultCode result, string details)
        {
            _result = new OperationResult(typeof(T).Name, result, details);
        }

        public abstract void Execute();
    }
}

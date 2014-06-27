using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{


    public abstract class ServiceOperation
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

        public ServiceOperation(string targetPath, Deployment deployment, NugetFeed feed)
        {
            _targetPath = targetPath;
            _deployment = deployment;
            _feed = feed;
        }

        protected void StoreResult<T>(T serviceOperation, ResultCode result, string details)
        {
            _result = new OperationResult(typeof(T).Name, result, details);
        }

        protected int ExecuteWithParams(string exePath, string exeParams)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = exePath;
            startInfo.Arguments = exeParams;
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            return process.ExitCode;
        }

        public abstract void Execute();
    }
}

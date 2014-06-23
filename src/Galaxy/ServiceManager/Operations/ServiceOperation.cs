using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager
{
    public enum OperationResult
    {
        OR_DEFAULT,
        OR_OK,
        OR_FAIL
    }

    public abstract class ServiceOperation
    {
        protected const string serviceHostFileName = "Codestellation.Galaxy.Host.exe";

        OperationResult _result;
        string _details;
        readonly protected string _targetPath;
        readonly protected NugetFeed _feed;
        readonly protected ServiceApp _serviceApp;

        public NugetFeed Feed
        {
            get { return _feed; }
        }
        public ServiceApp ServiceApp
        {
            get { return _serviceApp; }
        }
        public OperationResult Result
        {
            get { return _result; }
        }

        public ServiceOperation(string targetPath, ServiceApp serviceApp, NugetFeed feed)
        {
            this._targetPath = targetPath;
            this._serviceApp = serviceApp;
            this._feed = feed;
        }

        protected void StoreResult(OperationResult result, string details)
        {
            this._result = result;
            this._details = details;
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

using System;
using System.ComponentModel;
using System.IO;
using Codestellation.Galaxy.ServiceManager.Helpers;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallService : WinServiceOperation
    {
        private readonly string _serviceFolder;
        private readonly string _hostFileName;
        private readonly string _instance;

        public UninstallService(string serviceFolder, string hostFileName, string instance)
            : base(instance)
        {
            _serviceFolder = serviceFolder;
            _hostFileName = hostFileName;
            _instance = instance;
        }

        public override void Execute(DeploymentTaskContext context)
        {
            var exePath = new FileInfo(Path.Combine(_serviceFolder, _hostFileName));

            if (!exePath.Exists && SkipIfNotFound)
            {
                context.BuildLog.WriteLine("Service executable '{0}' not found. Uninstall skipped", exePath.FullName);
                return;
            }

            if (!IsServiceExists() && SkipIfNotFound)
            {
                context.BuildLog.WriteLine("Service '{0}' are not installed. Uninstall skipped", exePath.FullName);
                return;
            }

            var exeParams = string.Format("uninstall -instance:{0}", _instance);

            context.BuildLog.WriteLine("Executing '{0} {1}'", exePath, exeParams);

            string error = "";
            string result = "";

            try
            {
                result = ProcessStarter.ExecuteWithParams(exePath.FullName, exeParams, out error);
            }
            catch (Win32Exception ex)
            {
                if (!SkipIfNotFound)
                {
                    throw;
                }

                TryHandle(ex, out result);
            }

            context.BuildLog.WriteLine("Exe output:");
            context.BuildLog.WriteLine(result);

            context.BuildLog.WriteLine("Exe error:");
            context.BuildLog.WriteLine(error);
        }

        private void TryHandle(Win32Exception ex, out string result)
        {
            switch (ex.NativeErrorCode)
            {
                case (int)WinErrors.FileNotFound:
                    result = "Service executable wasn't found. Will be continued";
                    break;
                default:
                    throw new InvalidOperationException("Unexpected exception", ex);
            }
        }
    }
}
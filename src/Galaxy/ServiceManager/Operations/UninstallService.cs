using System;
using System.ComponentModel;
using System.IO;
using Codestellation.Galaxy.ServiceManager.Helpers;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallService : WinServiceOperation
    {
        protected override void ExecuteInternal(DeploymentTaskContext context)
        {
            var exePath = new FileInfo(Path.Combine((string)context.Folders.DeployFolder, context.ServiceFileName));

            if (!exePath.Exists)
            {
                context.BuildLog.WriteLine("Service executable '{0}' not found. Uninstall skipped", exePath.FullName);
                return;
            }

            if (!IsServiceExists())
            {
                context.BuildLog.WriteLine("Service '{0}' are not installed. Uninstall skipped", exePath.FullName);
                return;
            }

            var exeParams = $"uninstall -instance:{context.InstanceName}";

            context.BuildLog.WriteLine("Executing '{0} {1}'", exePath, exeParams);

            ExecutionResult result = null;
            string handledResult = null;

            try
            {
                result = ProcessStarter.ExecuteWithParams(exePath.FullName, exeParams);
            }
            catch (Win32Exception ex)
            {
                TryHandle(ex, out handledResult);
            }

            context.BuildLog.WriteLine("Exe output:");
            context.BuildLog.WriteLine(result);

            context.BuildLog.WriteLine("Exe error:");
            context.BuildLog.WriteLine(handledResult ?? result?.StdError);
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
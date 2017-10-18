using Codestellation.Galaxy.ServiceManager.Helpers;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager
{
    public class GetConfigSample : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            var exePath = Folder.Combine((string)context.Folders.DeployFolder, context.ServiceFileName);

            var arguments = "config-sample";
            context.BuildLog.WriteLine($"Executing: '{exePath} {arguments}'");

            var result = ProcessStarter.ExecuteWithParams(exePath, arguments, throwOnError: false);

            if (result.ExitCode == 0)
            {
                var sample = result.StdOut;
                context.BuildLog.WriteLine($"Completion result: {sample}");
                var request = new SetConfigSampleRequest(context.DeploymentId, sample);
                context.Mediator.Send(request);
            }
            else
            {
                context.BuildLog.WriteLine($"Completion error: {result.StdError}");
                context.BuildLog.WriteLine($"Completion output: {result.StdOut}");
                context.BuildLog.WriteLine($"May be service does not support command 'config-sample'?");
            }
        }
    }
}
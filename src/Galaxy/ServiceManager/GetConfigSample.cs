using Codestellation.Galaxy.ServiceManager.Helpers;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Quarks.IO;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager
{
    public class GetConfigSample : IOperation
    {
        private readonly string _serviceFolder;
        private readonly string _hostFileName;
        private DeploymentTaskContext _context;

        public GetConfigSample(string serviceFolder, string hostFileName)
        {
            _serviceFolder = serviceFolder;
            _hostFileName = hostFileName;
        }

        public void Execute(DeploymentTaskContext context)
        {
            _context = context;
            var exePath = Folder.Combine(_serviceFolder, _hostFileName);

            var arguments = "config-sample";
            context.BuildLog.WriteLine($"Executing: '{exePath} {arguments}'");

            var result = ProcessStarter.ExecuteWithParams(exePath, arguments, throwOnError: false);

            if (result.ExitCode == 0)
            {
                var sample = result.StdOut;
                context.BuildLog.WriteLine($"Completion result: {sample}");
                var request = new SetConfigSampleRequest(_context.GetValue<ObjectId>(DeploymentTaskContext.DeploymentId), sample);
                _context.Mediator.Send(request);
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
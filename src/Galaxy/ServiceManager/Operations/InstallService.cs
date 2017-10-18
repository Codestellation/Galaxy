using System.Collections.Generic;
using System.Linq;
using Codestellation.Galaxy.ServiceManager.Helpers;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallService : IOperation
    {
        private string _instance;

        public void Execute(DeploymentTaskContext context)
        {
            _instance = context.InstanceName;
            var exePath = Folder.Combine((string)context.Folders.DeployFolder, context.ServiceFileName);

            context.BuildLog.WriteLine("Executing '{0} {1}'", exePath, CommandLineArguments);

            var result = ProcessStarter.ExecuteWithParams(exePath, CommandLineArguments);

            context.BuildLog.WriteLine("Exe output:");
            context.BuildLog.WriteLine(result.StdOut);
            context.BuildLog.WriteLine("Exe error:");
            context.BuildLog.WriteLine(result.StdError);
        }

        private string CommandLineArguments => "install " + ArgumentsString;

        private string ArgumentsString
        {
            get
            {
                var arguments = Arguments.Select(x => $"-{x.Key}:{x.Value}");
                return string.Join(" ", arguments);
            }
        }

        private IEnumerable<KeyValuePair<string, object>> Arguments
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_instance))
                {
                    yield return new KeyValuePair<string, object>("instance", _instance);
                }
            }
        }
    }
}
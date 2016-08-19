using System.Collections.Generic;
using System.Linq;
using Codestellation.Galaxy.ServiceManager.Helpers;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallService : IOperation
    {
        private readonly string _serviceFolder;
        private readonly string _hostFileName;
        private readonly string _instance;
        private DeploymentTaskContext _context;

        public InstallService(string serviceFolder, string hostFileName, string instance)
        {
            _serviceFolder = serviceFolder;
            _hostFileName = hostFileName;
            _instance = instance;
        }

        public void Execute(DeploymentTaskContext context)
        {
            _context = context;
            var exePath = Folder.Combine(_serviceFolder, _hostFileName);

            context.BuildLog.WriteLine("Executing '{0} {1}'", exePath, CommandLineArguments);

            var result = ProcessStarter.ExecuteWithParams(exePath, CommandLineArguments);

            context.BuildLog.WriteLine("Exe output:");
            context.BuildLog.WriteLine(result.StdOut);
            context.BuildLog.WriteLine("Exe error:");
            context.BuildLog.WriteLine(result.StdError);
        }

        private string CommandLineArguments
        {
            get { return "install " + ArgumentsString; }
        }

        private string ArgumentsString
        {
            get
            {
                var arguments = Arguments.Select(x => string.Format("-{0}:{1}", x.Key, x.Value));
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
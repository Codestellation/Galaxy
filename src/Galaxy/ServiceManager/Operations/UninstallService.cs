using Codestellation.Galaxy.ServiceManager.Helpers;
using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallService : IOperation
    {
        private readonly string _serviceFolder;
        private readonly string _hostFileName;

        public UninstallService(string serviceFolder, string hostFileName)

        {
            _serviceFolder = serviceFolder;
            _hostFileName = hostFileName;
        }

        public void Execute(TextWriter buildLog)
        {
            var exePath = Path.Combine(_serviceFolder, _hostFileName);

            var exeParams = "uninstall";

            ProcessStarter.ExecuteWithParams(exePath, exeParams);
        }
    }
}

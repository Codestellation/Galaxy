using Codestellation.Galaxy.ServiceManager.Helpers;
using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallService : IOperation
    {
        private readonly string _serviceFolder;
        private readonly string _hostFileName;
        private readonly string _instance;

        public InstallService(string serviceFolder, string hostFileName, string instance)
        {
            _serviceFolder = serviceFolder;
            _hostFileName = hostFileName;
            _instance = instance;
        }

        public void Execute(TextWriter buildLog)
        {
            string exePath = Path.Combine(_serviceFolder, _hostFileName);

            string exeParams = string.Format("install -instance:{0}", _instance);

            string error;
            var result = ProcessStarter.ExecuteWithParams(exePath, exeParams, out error);

            buildLog.WriteLine("Exe output:");
            buildLog.WriteLine(result);

            buildLog.WriteLine("Exe error:");
            buildLog.WriteLine(error);
        }
    }
}
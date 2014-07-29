using System.IO;
using Codestellation.Galaxy.Infrastructure;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallPackage : IOperation
    {
        private readonly string _serviceFolder;

        public UninstallPackage(string serviceFolder)
        {
            _serviceFolder = serviceFolder;
        }

        public void Execute(TextWriter buildLog)
        {
            buildLog.WriteLine("Delete folder {0}", _serviceFolder);
            Folder.Delete(_serviceFolder);
        }
    }
}

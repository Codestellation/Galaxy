
using System.IO;
using System.Text;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public abstract class OperationBase : IOperation
    {
        protected const string ServiceHostFileName = "Codestellation.Galaxy.Host.exe";

        protected readonly Deployment Deployment;
        protected readonly string ServiceFolder;

        public OperationBase(string basePath, Deployment deployment)
        {
            Deployment = deployment;
            ServiceFolder = deployment.GetDeployFolder(basePath);
        }

        public abstract void Execute(TextWriter buildLog);
    }
}
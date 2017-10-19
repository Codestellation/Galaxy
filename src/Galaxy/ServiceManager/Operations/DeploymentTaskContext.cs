using System.Dynamic;
using System.IO;
using System.ServiceProcess;
using Codestellation.Galaxy.Domain;
using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeploymentTaskContext
    {
        public string TaskName { get; set; }

        public ObjectId DeploymentId { get; set; }
        public readonly TextWriter BuildLog;

        public string Config { get; set; }

        public dynamic Parameters { get; set; }
        public ServiceFolders Folders { get; set; }
        public FileList KeepOnUpdate { get; set; }
        public string ServiceFileName { get; set; }
        public string ServiceName { get; set; }
        public string InstanceName { get; set; }

        public ServiceControllerStatus? ServiceStatus { get; set; }

        public IMediator Mediator { get; set; }

        public PackageDetails PackageDetails { get; set; }
        public Stream LogStream { get; set; }

        public DeploymentTaskContext(TextWriter buildLog)
        {
            BuildLog = buildLog;
            Parameters = new ExpandoObject();
        }
    }
}
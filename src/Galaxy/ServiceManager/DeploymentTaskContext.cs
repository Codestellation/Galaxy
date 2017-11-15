using System;
using System.Dynamic;
using System.IO;
using System.ServiceProcess;
using System.Text;
using Codestellation.Galaxy.Domain;
using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeploymentTaskContext
    {
        public string TaskName { get; set; }

        public ObjectId DeploymentId { get; set; }
        public TextWriter BuildLog { get; }

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
        public Stream LogStream { get; }
        public Version InstalledPackageVersion { get; set; }

        public ServiceFolders NewFolders { get; set; }

        public DeploymentTaskContext()
        {
            LogStream = new MemoryStream(32 * 1024);
            BuildLog = new StreamWriter(LogStream, Encoding.UTF8);
            Parameters = new ExpandoObject();
        }
    }
}
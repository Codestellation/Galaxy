using System;
using System.Linq;
using System.ServiceProcess;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class Deployment
    {
        public ObjectId Id { get; internal set; }

        public string Group { get; set; }

        public string InstanceName { get; set; }

        public ObjectId FeedId { get; set; }
        public string Status { get; set; }
        public string PackageId { get; set; }
        public Version PackageVersion { get; set; }
        public FileList KeepOnUpdate { get; set; }

        public string ConfigSample { get; set; }

        public string Config { get; set; }

        public ServiceFolders Folders { get; set; }

        public bool HasInstanceName => !string.IsNullOrWhiteSpace(InstanceName);

        public Deployment()
        {
            Folders = new ServiceFolders();
        }

        //TODO: Think how to remove it
        public string GetServiceName()
        {
            //NOTE: instance name may change. Do not cache it
            return string.IsNullOrWhiteSpace(InstanceName) ? PackageId : $"{PackageId}${InstanceName}";
        }

        public string GetDisplayName()
        {
            var result = PackageId;
            if (!string.IsNullOrWhiteSpace(InstanceName))
            {
                result += $"{result} (Instance: {InstanceName})";
            }
            return result;
        }

        public string GetServiceState()
        {
            ServiceController[] services = ServiceController.GetServices();
            var controller = services.SingleOrDefault(item => item.ServiceName == GetServiceName());

            if (controller == null)
            {
                return "NotFound";
            }

            return controller.Status.ToString();
        }
    }
}
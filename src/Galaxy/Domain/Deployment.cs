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

        public string ConsulName { get; set; }

        public Deployment()
        {
            ServiceFolders = new SpecialFolderDictionary();
        }

        public SpecialFolderDictionary ServiceFolders { get; private set; }

        public bool HasInstanceName => !string.IsNullOrWhiteSpace(InstanceName);

        public string GetServiceName()
        {
            //NOTE: instance name may change. Do not cache it
            if (string.IsNullOrWhiteSpace(InstanceName))
            {
                return PackageId;
            }

            return string.Format("{0}${1}", PackageId, InstanceName);
        }

        public string GetDisplayName()
        {
            var result = PackageId;
            if (!string.IsNullOrWhiteSpace(InstanceName))
            {
                result += string.Format("{0} (Instance: {1})", result, InstanceName);
            }
            return result;
        }

        public string GetDeployFolder()
        {
            return ServiceFolders[SpecialFolderDictionary.DeployFolder].FullPath;
        }

        public string GetDeployLogFolder()
        {
            return ServiceFolders[SpecialFolderDictionary.DeployLogsFolder].FullPath;
        }

        public string GetBackupFolder()
        {
            return ServiceFolders[SpecialFolderDictionary.BackupFolder].FullPath;
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

        public string GetServiceHostFileName()
        {
            return string.Format("{0}.exe", PackageId);
        }
    }
}
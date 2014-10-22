﻿using System.Linq;
using System.ServiceProcess;
using Codestellation.Quarks.IO;
using Nejdb.Bson;
using System;

namespace Codestellation.Galaxy.Domain
{
    public class Deployment
    {
        private string _deployLogFolder;
        private string _fileOverridesFolder;
        private string _backupFolder;

        public ObjectId Id { get; internal set; }
        
        public string Group { get; set; }

        public string InstanceName { get; set; }

        public ObjectId FeedId { get; set; }
        public string Status { get; set; }
        public string PackageId { get; set; }
        public Version PackageVersion { get; set; }

        public FileList KeepOnUpdate { get; set; }

        public string GetServiceName()
        {
            if (string.IsNullOrWhiteSpace(InstanceName))
            {
                return PackageId;
            }

            return string.Format("{0}${1}", PackageId, InstanceName);
        }

        public string GetDeployFolder(string baseFolder)
        {
            if (string.IsNullOrWhiteSpace(InstanceName))
            {
                return Folder.Combine(baseFolder, PackageId);
            }

            return Folder.Combine(baseFolder, string.Format("{0}-{1}", PackageId, InstanceName));
        }

        public string GetDeployLogFolder()
        {
            return _deployLogFolder ?? (_deployLogFolder = BuildServiceFolder("BuildLogs"));
        }

        public string GetFilesFolder()
        {
            return _fileOverridesFolder ?? (_fileOverridesFolder = BuildServiceFolder("FileOverrides"));
        }

        public string GetBackupFolder()
        {
            return _backupFolder ?? (_backupFolder = BuildServiceFolder("Backups"));
        }

        private string BuildServiceFolder(string subfolder)
        {
            return Folder.Combine(Folder.BasePath, Id.ToString() , subfolder);
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

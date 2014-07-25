using System.IO;
using Codestellation.Galaxy.Infrastructure;
using Nejdb.Bson;
using System;

namespace Codestellation.Galaxy.Domain
{
    public class Deployment
    {
        public ObjectId Id { get; internal set; }
        public string AssemblyQualifiedType { get; set; }
        
        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
        public string InstanceName { get; set; }
        public string Description { get; set; }
        
        public ObjectId FeedId { get; set; }
        public string Status { get; set; }
        public string PackageId { get; set; }
        public Version PackageVersion { get; set; }

        public string ConfigFileContent { get; set; }

        public FileList KeepOnUpdate { get; set; }

        public string GetServiceName()
        {
            if (string.IsNullOrWhiteSpace(InstanceName))
            {
                return ServiceName;
            }

            return string.Format("{0}${1}", ServiceName, InstanceName);
        }

        public string GetDeployFolder(string baseFolder)
        {
            return Folder.Combine(baseFolder, DisplayName);
        }
    }
}

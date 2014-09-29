using System.IO;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class BackupListModel
    {
        public ObjectId DeploymentId { get; set; }
        
        public DirectoryInfo[] Folders { get; set; }

        public BackupListModel(ObjectId deploymentId, DirectoryInfo[] folders)
        {
            DeploymentId = deploymentId;
            Folders = folders;
        }
    }
}
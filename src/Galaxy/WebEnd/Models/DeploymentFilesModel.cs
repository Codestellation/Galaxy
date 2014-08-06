using System.IO;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class DeploymentFilesModel
    {
        public ObjectId DeploymentId { get; set; }
        
        public FileInfo[] Files { get; set; }

        public DeploymentFilesModel(ObjectId deploymentId, FileInfo[] files)
        {
            DeploymentId = deploymentId;
            Files = files;
        }
    }
}
using System.IO;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class BuildLogsModel
    {
        public ObjectId DeploymentId { get; set; }

        public FileInfo[] Files { get; set; }

        public BuildLogsModel(ObjectId deploymentId, FileInfo[] files)
        {
            DeploymentId = deploymentId;
            Files = files;
        }
    }
}
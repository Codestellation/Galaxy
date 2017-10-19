using System.IO;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class BuildLogsModel
    {
        public string DeploymentId { get; set; }

        public FileInfo[] Files { get; set; }

        public BuildLogsModel(ObjectId deploymentId, FileInfo[] files)
        {
            DeploymentId = deploymentId.ToString();
            Files = files;
        }
    }
}
using Codestellation.Galaxy.Infrastructure;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager
{
    public class ConfigSampleReceived : IMainRequest
    {
        public ObjectId DeploymentId { get; }
        public string Sample { get; }

        public ConfigSampleReceived(ObjectId deploymentId, string sample)
        {
            DeploymentId = deploymentId;
            Sample = sample;
        }
    }
}
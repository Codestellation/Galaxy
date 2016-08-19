using Codestellation.Galaxy.Infrastructure.Emisstar;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager
{
    [Synchronized]
    public class ConfigSampleReceived
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
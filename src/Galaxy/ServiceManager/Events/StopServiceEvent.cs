using Codestellation.Galaxy.Infrastructure.Emisstar;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    [Synchronized]
    public class StopServiceEvent
    {
        public readonly ObjectId DeploymentId;

        public StopServiceEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
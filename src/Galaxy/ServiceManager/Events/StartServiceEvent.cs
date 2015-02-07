using Codestellation.Galaxy.Infrastructure.Emisstar;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    [Synchronized]
    public class StartServiceEvent
    {
        public readonly ObjectId DeploymentId;

        public StartServiceEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
using Codestellation.Galaxy.Infrastructure.Emisstar;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    [Synchronized]
    public class UninstallServiceEvent
    {
        public readonly ObjectId DeploymentId;

        public UninstallServiceEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
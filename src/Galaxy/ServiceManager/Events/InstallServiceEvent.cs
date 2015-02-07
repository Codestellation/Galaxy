using Codestellation.Galaxy.Infrastructure.Emisstar;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    [Synchronized]
    public class InstallServiceEvent
    {
        public readonly ObjectId DeploymentId;

        public InstallServiceEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
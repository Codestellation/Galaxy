using Codestellation.Galaxy.Infrastructure;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class InstallServiceEvent : IMainRequest
    {
        public readonly ObjectId DeploymentId;

        public InstallServiceEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
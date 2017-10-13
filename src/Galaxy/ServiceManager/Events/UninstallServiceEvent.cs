using Codestellation.Galaxy.Infrastructure;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class UninstallServiceEvent : IMainRequest
    {
        public readonly ObjectId DeploymentId;

        public UninstallServiceEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
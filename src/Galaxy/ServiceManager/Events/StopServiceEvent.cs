using Codestellation.Galaxy.Infrastructure;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class StopServiceEvent : IMainRequest
    {
        public readonly ObjectId DeploymentId;

        public StopServiceEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
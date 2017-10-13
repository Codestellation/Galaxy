using Codestellation.Galaxy.Infrastructure;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeploymentDeletedEvent : IMainRequest
    {
        public readonly ObjectId DeploymentId;

        public DeploymentDeletedEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
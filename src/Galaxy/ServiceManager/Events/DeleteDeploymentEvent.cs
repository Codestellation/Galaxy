using Codestellation.Galaxy.Infrastructure;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeleteDeploymentEvent : IMainRequest
    {
        public readonly ObjectId DeploymentId;

        public DeleteDeploymentEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
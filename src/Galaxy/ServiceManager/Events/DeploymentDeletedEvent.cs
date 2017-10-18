using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeploymentDeletedEvent : IRequest
    {
        public readonly ObjectId DeploymentId;

        public DeploymentDeletedEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
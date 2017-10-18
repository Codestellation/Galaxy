using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeleteDeploymentRequest : IRequest
    {
        public readonly ObjectId DeploymentId;

        public DeleteDeploymentRequest(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
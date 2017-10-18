using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class StopServiceRequest : IRequest
    {
        public readonly ObjectId DeploymentId;

        public StopServiceRequest(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
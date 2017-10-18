using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class StartServiceRequest : IRequest
    {
        public readonly ObjectId DeploymentId;

        public StartServiceRequest(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
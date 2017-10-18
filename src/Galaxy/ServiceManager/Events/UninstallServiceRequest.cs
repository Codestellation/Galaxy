using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class UninstallServiceRequest : IRequest
    {
        public readonly ObjectId DeploymentId;

        public UninstallServiceRequest(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
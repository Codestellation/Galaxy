using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class InstallServiceRequest : IRequest
    {
        public readonly ObjectId DeploymentId;

        public InstallServiceRequest(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}
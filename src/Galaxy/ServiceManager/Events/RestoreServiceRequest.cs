using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class RestoreServiceRequest : IRequest
    {
        public ObjectId DeploymentId { get; }
        public string RestoreFrom { get; }

        public RestoreServiceRequest(ObjectId deploymentId, string restoreFrom)
        {
            DeploymentId = deploymentId;
            RestoreFrom = restoreFrom;
        }
    }
}
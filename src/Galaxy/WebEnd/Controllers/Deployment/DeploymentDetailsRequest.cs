using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Controllers.Deployment
{
    public class DeploymentDetailsRequest : IRequest<DeploymentDetailsResponse>
    {
        public ObjectId Id { get; }

        public DeploymentDetailsRequest(ObjectId id)
        {
            Id = id;
        }
    }
}
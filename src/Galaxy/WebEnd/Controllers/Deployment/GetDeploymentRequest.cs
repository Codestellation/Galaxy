using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Controllers.Deployment
{
    public class GetDeploymentRequest : IRequest<GetDeploymentResponse>
    {
        public ObjectId Id { get; }

        public GetDeploymentRequest(ObjectId id)
        {
            Id = id;
        }
    }
}
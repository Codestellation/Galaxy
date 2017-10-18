using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class EditDeploymentModelRequest : IRequest<EditDeploymentModelResponse>
    {
        public ObjectId Id { get; }

        public EditDeploymentModelRequest(ObjectId id)
        {
            Id = id;
        }
    }
}
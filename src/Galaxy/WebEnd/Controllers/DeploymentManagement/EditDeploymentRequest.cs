using Codestellation.Galaxy.WebEnd.Models.DeploymentManangement;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class EditDeploymentRequest : IRequest
    {
        public DeploymentEditModel Model { get; }

        public EditDeploymentRequest(DeploymentEditModel model)
        {
            Model = model;
        }
    }
}
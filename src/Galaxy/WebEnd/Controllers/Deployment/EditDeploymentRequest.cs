using Codestellation.Galaxy.WebEnd.Models.Deployment;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.Deployment
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
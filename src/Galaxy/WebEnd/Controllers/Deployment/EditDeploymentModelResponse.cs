using Codestellation.Galaxy.WebEnd.Models.Deployment;

namespace Codestellation.Galaxy.WebEnd.Controllers.Deployment
{
    public class EditDeploymentModelResponse
    {
        public DeploymentEditModel Model { get; }

        public EditDeploymentModelResponse(DeploymentEditModel model)
        {
            Model = model;
        }
    }
}
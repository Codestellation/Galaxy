using Codestellation.Galaxy.WebEnd.Models.DeploymentManangement;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
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
using Codestellation.Galaxy.WebEnd.Models.DeploymentManangement;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class DeploymentDetailsResponse
    {
        public DeploymentModel Model { get; }
        public static readonly DeploymentDetailsResponse Empty = new DeploymentDetailsResponse(null);

        public DeploymentDetailsResponse(DeploymentModel model)
        {
            Model = model;
        }
    }
}
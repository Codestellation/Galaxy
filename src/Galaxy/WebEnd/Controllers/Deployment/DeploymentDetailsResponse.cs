using Codestellation.Galaxy.WebEnd.Models.Deployment;

namespace Codestellation.Galaxy.WebEnd.Controllers.Deployment
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
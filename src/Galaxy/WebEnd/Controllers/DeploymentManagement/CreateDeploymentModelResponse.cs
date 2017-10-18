using System;
using Codestellation.Galaxy.WebEnd.Models.DeploymentManangement;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class CreateDeploymentModelResponse
    {
        public DeploymentEditModel Model { get; }

        public CreateDeploymentModelResponse(DeploymentEditModel model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }
    }
}
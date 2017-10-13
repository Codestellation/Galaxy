using System;
using Codestellation.Galaxy.WebEnd.Models.Deployment;

namespace Codestellation.Galaxy.WebEnd.Controllers.Deployment
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
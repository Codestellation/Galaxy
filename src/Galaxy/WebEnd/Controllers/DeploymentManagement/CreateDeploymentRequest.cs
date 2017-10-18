using System;
using Codestellation.Galaxy.WebEnd.Models.DeploymentManangement;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class CreateDeploymentRequest : IRequest
    {
        public DeploymentEditModel Model { get; }

        public CreateDeploymentRequest(DeploymentEditModel deployment)
        {
            Model = deployment ?? throw new ArgumentNullException(nameof(deployment));
        }
    }
}
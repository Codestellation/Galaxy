using System;

namespace Codestellation.Galaxy.WebEnd.Controllers.Deployment
{
    public class GetDeploymentResponse
    {
        public Domain.Deployment Deployment { get; }

        public GetDeploymentResponse(Domain.Deployment deployment)
        {
            Deployment = deployment ?? throw new ArgumentNullException(nameof(deployment));
        }
    }
}
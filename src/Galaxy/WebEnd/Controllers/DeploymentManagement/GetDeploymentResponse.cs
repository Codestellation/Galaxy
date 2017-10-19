using System;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class GetDeploymentResponse
    {
        public NugetFeed Feed { get; }
        public Deployment Deployment { get; }

        public GetDeploymentResponse(Deployment deployment, NugetFeed feed)
        {
            Deployment = deployment ?? throw new ArgumentNullException(nameof(deployment));
            Feed = feed ?? throw new ArgumentNullException(nameof(feed));
        }
    }
}
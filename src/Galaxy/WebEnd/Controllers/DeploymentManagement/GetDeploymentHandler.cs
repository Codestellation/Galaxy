using System;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class GetDeploymentHandler : IRequestHandler<GetDeploymentRequest, GetDeploymentResponse>
    {
        private readonly Repository _repository;

        public GetDeploymentHandler(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public GetDeploymentResponse Handle(GetDeploymentRequest message)
        {
            var deployment = _repository.Deployments.Load<Deployment>(message.Id);
            var feed = _repository
                .Feeds
                .PerformQuery<NugetFeed>()
                .Single(x => x.Id == deployment.FeedId);

            return new GetDeploymentResponse(deployment, feed);
        }
    }
}
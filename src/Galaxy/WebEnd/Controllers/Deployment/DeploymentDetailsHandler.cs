using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Models.Deployment;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.Deployment
{
    public class DeploymentDetailsHandler : IRequestHandler<DeploymentDetailsRequest, DeploymentDetailsResponse>
    {
        private readonly PackageBoard _board;
        private readonly Repository _repository;

        public DeploymentDetailsHandler(Repository repository, PackageBoard board)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _board = board ?? throw new ArgumentNullException(nameof(board));
        }

        public DeploymentDetailsResponse Handle(DeploymentDetailsRequest message)
        {
            var deployment = _repository.Deployments.Load<Domain.Deployment>(message.Id);
            if (deployment == null)
            {
                return DeploymentDetailsResponse.Empty;
            }

            NugetFeed[] feeds = _repository.Feeds.PerformQuery<NugetFeed>();
            NugetFeed feed = feeds.Single(x => x.Id == deployment.FeedId);
            IEnumerable<Version> versions = _board.GetPackageVersions(feed.Uri, deployment.PackageId);

            var model = new DeploymentModel(deployment, feeds, versions);
            return new DeploymentDetailsResponse(model);
        }
    }
}
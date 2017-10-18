using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Models.DeploymentManangement;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class DeploymentListHandler : IRequestHandler<DeploymentListRequest, DeploymentListResponse>
    {
        private readonly Repository _repository;

        public DeploymentListHandler(Repository repository)
        {
            _repository = repository;
        }

        public DeploymentListResponse Handle(DeploymentListRequest message)
        {
            var feeds = _repository.Feeds.PerformQuery<NugetFeed>();
            var deployments = _repository.Deployments.PerformQuery<Domain.Deployment>();

            return new DeploymentListResponse
            {
                Model = new DeploymentListModel(feeds, deployments)
            };
        }
    }
}
using System;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Models;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.FeedManagement
{
    public class FeedListHandler : IRequestHandler<FeedListRequest, FeedListResponse>
    {
        private readonly Repository _repository;

        public FeedListHandler(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public FeedListResponse Handle(FeedListRequest message)
        {
            NugetFeed[] feeds = _repository.Feeds.PerformQuery<NugetFeed>();
            var deployments = _repository.Deployments.PerformQuery<Domain.Deployment>();
            var model = new FeedListModel(feeds, deployments);
            return new FeedListResponse(model);
        }
    }
}
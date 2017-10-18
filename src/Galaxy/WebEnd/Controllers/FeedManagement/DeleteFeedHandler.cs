using System;
using Codestellation.Galaxy.Infrastructure;
using MediatR;
using Nejdb.Queries;

namespace Codestellation.Galaxy.WebEnd.Controllers.FeedManagement
{
    public class DeleteFeedHandler : IRequestHandler<DeleteFeedRequest, DeleteFeedResponse>
    {
        private readonly Repository _repository;

        public DeleteFeedHandler(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public DeleteFeedResponse Handle(DeleteFeedRequest message)
        {
            var criterion = Criterions.Field(nameof(Domain.Deployment.FeedId), Criterions.Equals(message.Id));

            var builder = new QueryBuilder(criterion);
            var feedInUse = _repository.Deployments.PerformQuery<Domain.Deployment>(builder).Length > 0;

            if (feedInUse)
            {
                return new DeleteFeedResponse("Feed use. Remove it from deployments to delete");
            }
            _repository.Feeds.Delete(message.Id);
            return new DeleteFeedResponse(null);
        }
    }
}
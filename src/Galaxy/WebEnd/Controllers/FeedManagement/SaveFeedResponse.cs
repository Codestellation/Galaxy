using System;
using Codestellation.Galaxy.Infrastructure;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.FeedManagement
{
    public class SaveFeedResponse : IRequestHandler<SaveFeedRequest>
    {
        private readonly Repository _repository;

        public SaveFeedResponse(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void Handle(SaveFeedRequest message)
        {
            using (var tx = _repository.Feeds.BeginTransaction())
            {
                var feed = message.Model.ToFeed();
                _repository.Feeds.Save(feed, false);
                tx.Commit();
            }
        }
    }
}
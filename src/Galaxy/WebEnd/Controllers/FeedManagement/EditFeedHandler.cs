using System;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.FeedManagement
{
    public class EditFeedHandler : IRequestHandler<EditFeedRequest>
    {
        private readonly Repository _repository;

        public EditFeedHandler(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void Handle(EditFeedRequest message)
        {
            using (var tx = _repository.Feeds.BeginTransaction())
            {
                var currentFeed = _repository.Feeds.Load<NugetFeed>(message.Model.Id);
                var updatedFeed = message.Model.ToFeed();
                currentFeed.Merge(updatedFeed);
                _repository.Feeds.Save(currentFeed, false);

                tx.Commit();
            }
        }
    }
}
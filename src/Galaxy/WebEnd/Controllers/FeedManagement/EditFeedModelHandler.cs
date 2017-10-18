using System;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Models;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.FeedManagement
{
    public class EditFeedModelHandler : IRequestHandler<EditFeedModelRequest, EditFeedModelResponse>
    {
        private readonly Repository _repository;

        public EditFeedModelHandler(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public EditFeedModelResponse Handle(EditFeedModelRequest message)
        {
            var feed = _repository.Feeds.Load<NugetFeed>(message.Id);
            var model = new FeedModel(feed, false);
            return new EditFeedModelResponse(model);
        }
    }
}
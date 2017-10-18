using Codestellation.Galaxy.WebEnd.Models;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.FeedManagement
{
    public class EditFeedRequest : IRequest
    {
        public FeedModel Model { get; }

        public EditFeedRequest(FeedModel model)
        {
            Model = model;
        }
    }
}
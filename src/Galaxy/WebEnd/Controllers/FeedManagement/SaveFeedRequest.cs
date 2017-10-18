using Codestellation.Galaxy.WebEnd.Models;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.FeedManagement
{
    public class SaveFeedRequest : IRequest
    {
        public FeedModel Model { get; }

        public SaveFeedRequest(FeedModel model)
        {
            Model = model;
        }
    }
}
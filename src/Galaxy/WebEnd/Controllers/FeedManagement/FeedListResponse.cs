using Codestellation.Galaxy.WebEnd.Models;

namespace Codestellation.Galaxy.WebEnd.Controllers.FeedManagement
{
    public class FeedListResponse
    {
        public FeedListModel Model { get; }

        public FeedListResponse(FeedListModel model)
        {
            Model = model;
        }
    }
}
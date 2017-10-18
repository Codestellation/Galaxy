using Codestellation.Galaxy.WebEnd.Models;

namespace Codestellation.Galaxy.WebEnd.Controllers.FeedManagement
{
    public class EditFeedModelResponse
    {
        public FeedModel Model { get; }

        public EditFeedModelResponse(FeedModel model)
        {
            Model = model;
        }
    }
}
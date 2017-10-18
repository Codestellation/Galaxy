namespace Codestellation.Galaxy.WebEnd.Controllers.FeedManagement
{
    public class DeleteFeedResponse
    {
        public string Error { get; }

        public DeleteFeedResponse(string error)
        {
            Error = error;
        }
    }
}
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class FeedListModel 
    {
        private FeedModel[] _feeds;

        public FeedListModel(DashBoard dashBoard)
        {
            _feeds = new FeedModel[dashBoard.FeedCount];
            int index = 0;
            foreach (var feed in dashBoard.Feeds)
            {
                var model = new FeedModel(feed);
                _feeds[index] = model;
                index++;
            }
        }

        public FeedModel[] Feeds
        {
            get { return _feeds; }
        }

        public int FeedCount
        {
            get { return _feeds.Length; }

        }
    }
}
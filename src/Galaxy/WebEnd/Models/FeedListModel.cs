using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.Collections;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class FeedListModel 
    {
        private FeedModel[] _feeds;

        public FeedListModel(DashBoard dashBoard)
        {
            _feeds = dashBoard.Feeds.ConvertToArray(x =>
            {
                var inUse = dashBoard.Deployments.Any(deployment => deployment.FeedId.Equals(x.Id));
                return new FeedModel(x, inUse);
            }, dashBoard.Feeds.Count);
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
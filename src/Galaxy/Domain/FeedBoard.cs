using System.Collections.Generic;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class FeedBoard
    {
        private readonly Dictionary<ObjectId, NugetFeed> _feeds;
        

        public FeedBoard()
        {
            _feeds = new Dictionary<ObjectId, NugetFeed>();
            
        }

        public IReadOnlyCollection<NugetFeed> Feeds
        {
            get { return new List<NugetFeed>(_feeds.Values); }
        }

        public void AddFeed(NugetFeed feed)
        {
            _feeds.Add(feed.Id, feed);
        }

        public void RemoveFeed(ObjectId id)
        {
            _feeds.Remove(id);
        }

        public NugetFeed GetFeed(ObjectId id)
        {
            return _feeds[id];
        }
    }
}
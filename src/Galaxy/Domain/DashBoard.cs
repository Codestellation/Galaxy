using System.Collections.Generic;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class DashBoard
    {
        private readonly Dictionary<ObjectId, NugetFeed> _feeds;
        
        public DashBoard()
        {
            _feeds = new Dictionary<ObjectId, NugetFeed>();
        }

        public IEnumerable<NugetFeed> Feeds
        {
            get { return _feeds.Values; }
        }

        public int FeedCount
        {
            get { return _feeds.Count; }
        }

        public void Add(NugetFeed feed)
        {
            _feeds.Add(feed.Id, feed);
        }

        public NugetFeed this[ObjectId id]
        {
            get { return _feeds[id]; }
        }

        public void Remove(ObjectId id)
        {
            _feeds.Remove(id);
        }
    }
}
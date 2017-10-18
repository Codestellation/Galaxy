using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.Collections;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class FeedListModel
    {
        private readonly FeedModel[] _feeds;

        public FeedListModel(NugetFeed[] feeds, Domain.Deployment[] deployments)
        {
            _feeds = feeds.ConvertToArray(
                x =>
                {
                    var inUse = deployments.Any(deployment => deployment.FeedId.Equals(x.Id));
                    return new FeedModel(x, inUse);
                });
        }

        public FeedModel[] Feeds => _feeds;

        public int FeedCount => _feeds.Length;
    }
}
using System.Collections.Generic;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class DashBoard
    {
        private readonly Dictionary<ObjectId, NugetFeed> _feeds;
        private readonly Dictionary<ObjectId, ServiceApp> _deployments;

        public DashBoard()
        {
            _feeds = new Dictionary<ObjectId, NugetFeed>();
            _deployments = new Dictionary<ObjectId, ServiceApp>();
        }

        public IReadOnlyCollection<NugetFeed> Feeds
        {
            get { return new List<NugetFeed>(_feeds.Values); }
        }

        public IReadOnlyCollection<ServiceApp> Deployments
        {
            get { return new List<ServiceApp>(_deployments.Values); }
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

        public void AddDeployment(ServiceApp serviceApp)
        {
            _deployments.Add(serviceApp.Id, serviceApp);
        }

        public ServiceApp GetDeployment(ObjectId id)
        {
            return _deployments[id];
        }

        public void RemoveDeployment(ObjectId id)
        {
            _deployments.Remove(id);
        }
    }
}
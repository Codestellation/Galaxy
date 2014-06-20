using System.Collections.Generic;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Nejdb;
using Nejdb.Bson;
using Nancy.Responses;
using Nancy.ModelBinding;
using Codestellation.Galaxy.WebEnd.Models;

namespace Codestellation.Galaxy.WebEnd
{
    public class ServiceModule : CrudModule
    {
        private readonly Collection _serviceApps;
        private readonly Collection _feeds;
        public const string Path = "service";

        public ServiceModule(Collections collections)
            : base(Path)
        {
            _serviceApps = collections.ServiceApps;
            _feeds = collections.Feeds;
        }

        protected override CrudOperations SupportedOperations
        {
            get { return CrudOperations.GetList | CrudOperations.GetCreate | CrudOperations.PostCreate | CrudOperations.GetEdit | CrudOperations.PostEdit | CrudOperations.PostDelete; }
        }

        protected override object GetList(dynamic parameters)
        {
            var queryResults = _serviceApps.PerformQuery<ServiceApp>();
            return View["list", queryResults];
        }

        protected override object GetCreate(dynamic parameters)
        {
            var allFeeds = GetAvailableFeeds();

            return View["edit", new ServiceAppModel(allFeeds)];
        }

        protected override object PostCreate(dynamic parameters)
        {
            var item = this.Bind<ServiceAppModel>();
            var serviceApp = item.ToServiceApp();

            using (var tx = _serviceApps.BeginTransaction())
            {
                _serviceApps.Save<ServiceApp>(serviceApp, false);
                tx.Commit();
            }

            return new RedirectResponse("/service");
        }

        protected override object GetEdit(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var item = _serviceApps.Load<ServiceApp>(id);

            return View["Edit", new ServiceAppModel(item, GetAvailableFeeds())];
        }

        protected override object PostEdit(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var updatedItem = this.Bind<ServiceAppModel>();
            var serviceApp = _serviceApps.Load<ServiceApp>(id);

            updatedItem.Update(serviceApp);

            using (var tx = _serviceApps.BeginTransaction())
            {
                _serviceApps.Save<ServiceApp>(serviceApp, false);
                tx.Commit();
            }

            return new RedirectResponse("/service");
        }

        protected override object PostDelete(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            _serviceApps.Delete(id);

            return new RedirectResponse("/service");
        }

        private KeyValuePair<ObjectId, string>[] GetAvailableFeeds()
        {
            var avaliableFeeds = _feeds.PerformQuery<NugetFeed>();

            var allFeeds = avaliableFeeds
                .Select(feed => new KeyValuePair<ObjectId, string>(feed.Id, feed.Name))
                .ToArray();
            return allFeeds;
        }
    }
}

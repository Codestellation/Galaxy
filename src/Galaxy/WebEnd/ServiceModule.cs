using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using Nejdb;
using Nejdb.Bson;
using Nancy.Responses;
using Nancy.ModelBinding;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.WebEnd.Models;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager;

namespace Codestellation.Galaxy.WebEnd
{
    public class ServiceModule : CrudModule
    {
        private readonly DashBoard _dashBoard;
        private readonly Collection _serviceApps;
        public const string Path = "service";

        public ServiceModule(Collections collections, DashBoard dashBoard)
            : base(Path)
        {
            _dashBoard = dashBoard;
            _serviceApps = collections.ServiceApps;

            Post["/install/{id}", true] = (parameters, token) => ProcessRequest(() => PostInstall(parameters), token);
            Post["/start/{id}", true] = (parameters, token) => ProcessRequest(() => PostStart(parameters), token);
            Post["/stop/{id}", true] = (parameters, token) => ProcessRequest(() => PostStop(parameters), token);
            Post["/uninstall/{id}", true] = (parameters, token) => ProcessRequest(() => PostUninstall(parameters), token);
        }

        protected override CrudOperations SupportedOperations
        {
            get { return CrudOperations.GetList | CrudOperations.GetCreate | CrudOperations.PostCreate | CrudOperations.GetEdit | CrudOperations.PostEdit | CrudOperations.PostDelete | CrudOperations.GetDetails; }
        }

        protected override object GetList(dynamic parameters)
        {
            var queryResults = _dashBoard.Deployments;
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
                _serviceApps.Save(serviceApp, false);
                tx.Commit();
            }

            _dashBoard.AddDeployment(serviceApp);

            return new RedirectResponse("/service");
        }

        protected override object GetEdit(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var item = _dashBoard.GetDeployment(id);

            return View["Edit", new ServiceAppModel(item, GetAvailableFeeds())];
        }

        protected override object PostEdit(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var updatedItem = this.Bind<ServiceAppModel>();

            var serviceApp = _dashBoard.GetDeployment(id);

            updatedItem.Update(serviceApp);

            using (var tx = _serviceApps.BeginTransaction())
            {
                _serviceApps.Save(serviceApp, false);
                tx.Commit();
            }

            return new RedirectResponse("/service");
        }

        protected override object PostDelete(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            _serviceApps.Delete(id);
            _dashBoard.RemoveDeployment(id);

            return new RedirectResponse("/service");
        }

        protected override object GetDetails(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var item = _dashBoard.GetDeployment(id);

            return View["details", new ServiceAppModel(item, null)];
        }
		
        private KeyValuePair<ObjectId, string>[] GetAvailableFeeds()
        {
            var allFeeds = _dashBoard.Feeds
                .Select(feed => new KeyValuePair<ObjectId, string>(feed.Id, feed.Name))
                .ToArray();
            return allFeeds;
        }		

        private object PostInstall(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var serviceApp = _dashBoard.GetDeployment(id);
          
            var targetFeed = _dashBoard.Feeds.FirstOrDefault(item => item.Name == serviceApp.FeedName);

            if(targetFeed != null)
            {
                string targetPath = ConfigurationManager.AppSettings["appsdestination"];
                string hostPackageFeedUri = ConfigurationManager.AppSettings["hostPackageFeedUri"];
                string hostPackageName = ConfigurationManager.AppSettings["hostPackageName"];

                ServiceControl srvCtrl = new ServiceControl(targetPath, serviceApp, targetFeed);
                srvCtrl.AddInstall(
                    new ServiceApp
                    {
                        DisplayName = serviceApp.DisplayName,
                        PackageName = hostPackageName
                    },
                    new NugetFeed
                    {
                        Name = hostPackageName,
                        Uri = hostPackageFeedUri
                    });

                srvCtrl.OnCompleted += nugetMan_OnOperationCompleted;
                srvCtrl.Operate();
            }

            return new RedirectResponse(string.Format("/service/details/{0}", serviceApp.Id));
        }

        private object PostStart(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var serviceApp = _dashBoard.GetDeployment(id);
          
            var targetFeed = _dashBoard.Feeds.FirstOrDefault(item => item.Name == serviceApp.FeedName);

            if (targetFeed != null)
            {
                string targetPath = ConfigurationManager.AppSettings["appsdestination"];
                ServiceControl srvCtrl = new ServiceControl(targetPath, serviceApp, targetFeed);
                srvCtrl.AddStart();
                srvCtrl.OnCompleted += nugetMan_OnOperationCompleted;
                srvCtrl.Operate();
            }

            return new RedirectResponse(string.Format("/service/details/{0}", serviceApp.Id));
        }
        private object PostStop(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var serviceApp = _dashBoard.GetDeployment(id);

            var targetFeed = _dashBoard.Feeds.FirstOrDefault(item => item.Name == serviceApp.FeedName);

            if (targetFeed != null)
            {
                string targetPath = ConfigurationManager.AppSettings["appsdestination"];
                ServiceControl srvCtrl = new ServiceControl(targetPath, serviceApp, targetFeed);

                srvCtrl.AddStop();
                srvCtrl.OnCompleted += nugetMan_OnOperationCompleted;
                srvCtrl.Operate();
            }

            return new RedirectResponse(string.Format("/service/details/{0}", serviceApp.Id));
        }
        
        private object PostUninstall(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var serviceApp = _dashBoard.GetDeployment(id);

            var targetFeed = _dashBoard.Feeds.FirstOrDefault(item => item.Name == serviceApp.FeedName);

            if (targetFeed != null)
            {
                string targetPath = ConfigurationManager.AppSettings["appsdestination"];
                ServiceControl srvCtrl = new ServiceControl(targetPath, serviceApp, targetFeed);

                srvCtrl.AddUninstall();
                srvCtrl.OnCompleted += nugetMan_OnOperationCompleted;
                srvCtrl.Operate();
            }

            return new RedirectResponse(string.Format("/service/details/{0}", serviceApp.Id));            
        }

        void nugetMan_OnOperationCompleted(object sender, ServiceManager.EventParams.OperationCompletedEventArgs e)
        {
            var serviceApp = _dashBoard.GetDeployment(e.ServiceApp.Id);
            serviceApp.Status = e.Result.ToString();

            using (var tx = _serviceApps.BeginTransaction())
            {
                _serviceApps.Save(serviceApp, false);
                tx.Commit();
            }
        }
    }
}

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
using System;

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
            return View["list", new DeploymentListModel(_dashBoard)];
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

            ExecuteServiceControlAction(id, (srvCtrl) => srvCtrl.AddInstall());

            return new RedirectResponse(string.Format("/service/details/{0}", id));   
        }

        private object PostStart(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, (srvCtrl) => srvCtrl.AddStart());

            return new RedirectResponse(string.Format("/service/details/{0}", id)); 
        }
        private object PostStop(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, (srvCtrl) => srvCtrl.AddStop());

            return new RedirectResponse(string.Format("/service/details/{0}", id));
        }
        
        private object PostUninstall(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, (srvCtrl) => srvCtrl.AddUninstall());

            return new RedirectResponse(string.Format("/service/details/{0}", id));            
        }


        void ExecuteServiceControlAction(ObjectId serviceAppId, Action<ServiceControl> customAction)
        {
            var serviceApp = _dashBoard.GetDeployment(serviceAppId);

            var targetFeed = _dashBoard.Feeds.FirstOrDefault(item => item.Name == serviceApp.FeedName);

            if (targetFeed != null)
            {
                ServiceControl srvCtrl = new ServiceControl(serviceApp, targetFeed);
                customAction(srvCtrl);
                srvCtrl.OnCompleted += srvCtrl_OnOperationCompleted;
                srvCtrl.Operate();
            }
        }

        void srvCtrl_OnOperationCompleted(object sender, ServiceManager.EventParams.OperationCompletedEventArgs e)
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

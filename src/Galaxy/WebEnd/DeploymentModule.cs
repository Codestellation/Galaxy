using System.Linq;
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
using Codestellation.Galaxy.ServiceManager.Helpers;
using Codestellation.Galaxy.ServiceManager.EventParams;
using System.Threading;

namespace Codestellation.Galaxy.WebEnd
{
    public class DeploymentModule : CrudModule
    {
        private readonly DashBoard _dashBoard;
        private readonly Collection _deployments;
        public const string Path = "deployment";

        public DeploymentModule(Collections collections, DashBoard dashBoard)
            : base(Path)
        {
            _dashBoard = dashBoard;
            _deployments = collections.Deployments;

            Post["/install/{id}", true] = (parameters, token) => ProcessRequest(() => PostInstall(parameters), token);
            Post["/start/{id}", true] = (parameters, token) => ProcessRequest(() => PostStart(parameters), token);
            Post["/stop/{id}", true] = (parameters, token) => ProcessRequest(() => PostStop(parameters), token);
            Post["/uninstall/{id}", true] = (parameters, token) => ProcessRequest(() => PostUninstall(parameters), token);
            Post["/deploy/{id}", true] = (parameters, token) => ProcessRequest(() => PostDeploy(parameters), token);
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

            return View["edit", new DeploymentModel(allFeeds)];
        }

        protected override object PostCreate(dynamic parameters)
        {
            var item = this.Bind<DeploymentModel>();
            var deployment = item.ToDeployment();

            SaveDeployment(deployment);

            _dashBoard.AddDeployment(deployment);

            return RedirectToList();
        }

        protected override object GetEdit(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var deployment = _dashBoard.GetDeployment(id);

            return View["Edit", new DeploymentModel(deployment, GetAvailableFeeds())];
        }

        protected override object PostEdit(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var updatedItem = this.Bind<DeploymentModel>();

            var deployment = _dashBoard.GetDeployment(id);

            updatedItem.Update(deployment);

            SaveDeployment(deployment);

            return new RedirectResponse("/" + Path);
        }

        protected override object PostDelete(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            _deployments.Delete(id);
            _dashBoard.RemoveDeployment(id);

            return new RedirectResponse("/" + Path);
        }

        protected override object GetDetails(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var deployment = _dashBoard.GetDeployment(id);
            var feed = _dashBoard.GetFeed(deployment.FeedId);

            var versions = _dashBoard.VersionCache.GetPackageVersions(deployment.PackageName);

            return View["details", new DeploymentModel(deployment, GetAvailableFeeds(), versions)];
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

            ExecuteServiceControlAction(id, (deployment, feed) => DeploymentTaskBuilder.InstallServiceTask(deployment, feed));         

            return RedirectToDetails(id);
        }

        private object PostStart(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, (deployment, feed) => DeploymentTaskBuilder.StartServiceTask(deployment, feed));         

            return RedirectToDetails(id);
        }

        private object PostStop(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, (deployment, feed) => DeploymentTaskBuilder.StopServiceTask(deployment, feed));         

            return RedirectToDetails(id);
        }

        private object PostUninstall(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, (deployment, feed) => DeploymentTaskBuilder.UninstallServiceTask(deployment, feed));         

            return RedirectToDetails(id);
        }


        void ExecuteServiceControlAction(ObjectId deploymentId, Func<Deployment, NugetFeed, DeploymentTask> taskFunc)
        {
            var deployment = _dashBoard.GetDeployment(deploymentId);

            var targetFeed = _dashBoard.Feeds.FirstOrDefault(item => item.Id.Equals(deployment.FeedId));            

            if (targetFeed != null)
            {
                new DeploymentProcessor().Process(taskFunc(deployment, targetFeed), DeploymentCallbackFunc);
            }
        }

        void DeploymentCallbackFunc(object sender, DeploymentTaskCompletedEventArgs e)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            ProcessRequest(() =>
            {
                var deployment = _dashBoard.GetDeployment(e.Task.Deployment.Id);
                deployment.Status = e.Result.Details;

                SaveDeployment(deployment);

                return null;
            }, cts.Token);
        }

        private object PostDeploy(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var deploymentModel = this.Bind<DeploymentModel>();

            var deployment = _dashBoard.GetDeployment(id);
            deployment.PackageVersion = deploymentModel.PackageVersion;

            SaveDeployment(deployment);

            ExecuteServiceControlAction(id, (deploymentItem, feed) => DeploymentTaskBuilder.DeployServiceTask(deploymentItem, feed));

            return RedirectToDetails(id);
        }

        private void SaveDeployment(Deployment deployment)
        {
            using (var tx = _deployments.BeginTransaction())
            {
                _deployments.Save(deployment, false);
                tx.Commit();
            }
        }
        
        private static object RedirectToList()
        {
            return new RedirectResponse("/" + Path);
        }

        private static object RedirectToDetails(ObjectId id)
        {
            var detailsPath = string.Format("/{0}/details/{1}", Path, id);
            return new RedirectResponse(detailsPath);
        }

    }
}

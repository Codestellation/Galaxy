using System.Linq;
using System.Collections.Generic;
using Codestellation.Galaxy.Domain.Deployments;
using Codestellation.Galaxy.WebEnd.Models.Deployment;
using Codestellation.Quarks.Collections;
using Codestellation.Quarks.IO;
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
    public class DeploymentModule : CrudModule
    {
        private readonly FeedBoard _feedBoard;
        private readonly PackageVersionBoard _versionCache;
        private readonly TaskBuilder _taskBuilder;
        
        private readonly DeploymentBoard _deploymentBoard;
        public const string Path = "deployment";

        public DeploymentModule(FeedBoard feedBoard, PackageVersionBoard versionCache, TaskBuilder taskBuilder, DeploymentBoard deploymentBoard)
            : base(Path)
        {
            _feedBoard = feedBoard;
            _versionCache = versionCache;
            _taskBuilder = taskBuilder;
            _deploymentBoard = deploymentBoard;

            Post["/install/{id}", true] = (parameters, token) => ProcessRequest(() => PostInstall(parameters), token);
            Post["/start/{id}", true] = (parameters, token) => ProcessRequest(() => PostStart(parameters), token);
            Post["/stop/{id}", true] = (parameters, token) => ProcessRequest(() => PostStop(parameters), token);
            Post["/uninstall/{id}", true] = (parameters, token) => ProcessRequest(() => PostUninstall(parameters), token);
            Post["/deploy/{id}/{version}", true] = (parameters, token) => ProcessRequest(() => PostDeploy(parameters), token);
            Post["/update/{id}/{version}", true] = (parameters, token) => ProcessRequest(() => PostUpdate(parameters), token);

            Get["/build-log/{id}", true] = (parameters, token) => ProcessRequest(() => GetBuildLogs(parameters), token);
            Get["/build-log/{id}/{filename}", true] = (parameters, token) => ProcessRequest(() => GetBuildLog(parameters), token);

        }

        private object GetBuildLog(dynamic parameters)
        {
            Deployment deployment = GetDeployment(parameters);
            string filename = parameters.filename;

            var fullPath = System.IO.Path.Combine(deployment.GetDeployLogFolder(), filename);

            return new FileResponse(fullPath);
        }

        private object GetBuildLogs(dynamic parameters)
        {
            Deployment deployment = GetDeployment(parameters);

            var logFolder = deployment.GetDeployLogFolder();

            var files = Folder
                .EnumerateFiles(logFolder)
                .SortDescending(x => x.LastWriteTime);

            return new BuildLogsModel(deployment.Id, files);
        }

        protected override CrudOperations SupportedOperations
        {
            get { return CrudOperations.GetList | CrudOperations.GetCreate | CrudOperations.PostCreate | CrudOperations.GetEdit | CrudOperations.PostEdit | CrudOperations.PostDelete | CrudOperations.GetDetails; }
        }

        protected override object GetList(dynamic parameters)
        {
            return View["list", new DeploymentListModel(_feedBoard, _deploymentBoard)];
        }

        protected override object GetCreate(dynamic parameters)
        {
            var allFeeds = GetAvailableFeeds();

            return View["edit", new DeploymentEditModel { AllFeeds = allFeeds }];
        }

        protected override object PostCreate(dynamic parameters)
        {
            var item = this.Bind<DeploymentEditModel>();
            var deployment = item.ToDeployment();

            _deploymentBoard.AddDeployment(deployment);

            _versionCache.ForceRefresh();

            return RedirectToList();
        }

        protected override object GetEdit(dynamic parameters)
        {
            var deployment = GetDeployment(parameters);

            var deploymentEditModel = new DeploymentEditModel(deployment)
            {
                AllFeeds = GetAvailableFeeds()
            };
            return View["Edit", deploymentEditModel];
        }

        protected override object PostEdit(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var model = this.Bind<DeploymentEditModel>();

            var deployment = _deploymentBoard.GetDeployment(id);

            model.Update(deployment);

            _deploymentBoard.SaveDeployment(deployment);

            _versionCache.ForceRefresh();

            return RedirectToDetails(id);
        }

        protected override object PostDelete(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            _deploymentBoard.RemoveDeployment(id);

            return RedirectToList();
        }

        protected override object GetDetails(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var deployment = _deploymentBoard.GetDeployment(id);

            var versions = _versionCache.GetPackageVersions(deployment.FeedId, deployment.PackageId);

            return View["details", new DeploymentModel(deployment, GetAvailableFeeds(), versions)];
        }

        private KeyValuePair<ObjectId, string>[] GetAvailableFeeds()
        {
            var allFeeds =
                _feedBoard
                .Feeds
                .ConvertToArray(feed => new KeyValuePair<ObjectId, string>(feed.Id, feed.Name), _feedBoard.Feeds.Count);

            return allFeeds;
        }

        private object PostInstall(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, _taskBuilder.InstallServiceTask);

            return RedirectToDetails(id);
        }

        private object PostStart(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, _taskBuilder.StartServiceTask);

            return RedirectToDetails(id);
        }

        private object PostStop(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, _taskBuilder.StopServiceTask);

            return RedirectToDetails(id);
        }

        private object PostUninstall(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, _taskBuilder.UninstallServiceTask);

            return RedirectToDetails(id);
        }

        private void ExecuteServiceControlAction(ObjectId deploymentId, Func<Deployment, NugetFeed, DeploymentTask> taskFactory)
        {
            var deployment = _deploymentBoard.GetDeployment(deploymentId);

            var targetFeed = _feedBoard.Feeds.FirstOrDefault(item => item.Id.Equals(deployment.FeedId));

            //TODO: What else? Ignore silently? 
            if (targetFeed != null)
            {
                var task = taskFactory(deployment, targetFeed);
                task.Process();
            }
        }

        private object PostDeploy(dynamic parameters)
        {
            return PerformUpdateOrDeploy((Func<Deployment, NugetFeed, DeploymentTask>) _taskBuilder.DeployServiceTask, parameters);
        }

        private object PostUpdate(dynamic parameters)
        {
            return PerformUpdateOrDeploy((Func<Deployment, NugetFeed, DeploymentTask>)_taskBuilder.UpdateServiceTask, parameters);
        }

        private object PerformUpdateOrDeploy(Func<Deployment, NugetFeed, DeploymentTask> deployServiceTask, dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var version = new Version(parameters.version);

            var deployment = _deploymentBoard.GetDeployment(id);
            deployment.PackageVersion = version;

            _deploymentBoard.SaveDeployment(deployment);


            ExecuteServiceControlAction(id, deployServiceTask);

            return RedirectToDetails(id);
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

        private Deployment GetDeployment(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var deployment = _deploymentBoard.GetDeployment(id);
            return deployment;
        }
    }
}

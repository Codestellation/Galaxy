using System.Linq;
using System.Collections.Generic;
using Codestellation.Quarks.Collections;
using Codestellation.Quarks.IO;
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
    public class DeploymentModule : CrudModule
    {
        private readonly DashBoard _dashBoard;
        private readonly PackageVersionCache _versionCache;
        private readonly TaskBuilder _taskBuilder;
        private readonly Options _options;
        private readonly Collection _deployments;
        public const string Path = "deployment";

        public DeploymentModule(Repository repository, DashBoard dashBoard, PackageVersionCache versionCache, TaskBuilder taskBuilder, Options options)
            : base(Path)
        {
            _dashBoard = dashBoard;
            _versionCache = versionCache;
            _taskBuilder = taskBuilder;
            _options = options;
            _deployments = repository.GetCollection<Deployment>();

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

            FillServiceFolders(deployment);

            SaveDeployment(deployment);

            _dashBoard.AddDeployment(deployment);

            return RedirectToList();
        }

        private void FillServiceFolders(Deployment deployment)
        {
            var serviceFolders = deployment.ServiceFolders;

            var fullPath = string.IsNullOrWhiteSpace(deployment.InstanceName)
                ? Folder.Combine(_options.GetDeployFolder(), deployment.PackageId)
                : Folder.Combine(_options.GetDeployFolder(), string.Format("{0}-{1}", deployment.PackageId, deployment.InstanceName));
            var specialFolder = new SpecialFolder(SpecialFolderDictionary.DeployFolder, fullPath);
            serviceFolders.Add(specialFolder);


            BuildServiceFolder(deployment, SpecialFolderDictionary.DeployLogsFolder, "BuildLogs");
            BuildServiceFolder(deployment, SpecialFolderDictionary.FileOverrides, "FileOverrides");
            BuildServiceFolder(deployment, SpecialFolderDictionary.BackupFolder, "Backups");
        }

        private void BuildServiceFolder(Deployment deployment, string purpose, string subfolder)
        {
            var fullPath = Folder.Combine(Folder.BasePath, deployment.Id.ToString(), subfolder);
            var folder = new SpecialFolder(purpose, fullPath);
            deployment.ServiceFolders.Add(folder);
        }

        protected override object GetEdit(dynamic parameters)
        {
            var deployment = GetDeployment(parameters);

            return View["Edit", new DeploymentModel(deployment, GetAvailableFeeds())];
        }

        protected override object PostEdit(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var updatedItem = this.Bind<DeploymentModel>();

            var deployment = _dashBoard.GetDeployment(id);

            updatedItem.Update(deployment);

            SaveDeployment(deployment);

            return RedirectToDetails(id);
        }

        protected override object PostDelete(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            _deployments.Delete(id);
            _dashBoard.RemoveDeployment(id);

            return RedirectToList();
        }

        protected override object GetDetails(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var deployment = _dashBoard.GetDeployment(id);

            var versions = _versionCache.GetPackageVersions(deployment.FeedId, deployment.PackageId);

            return View["details", new DeploymentModel(deployment, GetAvailableFeeds(), versions)];
        }

        private KeyValuePair<ObjectId, string>[] GetAvailableFeeds()
        {
            var allFeeds =
                _dashBoard
                .Feeds
                .ConvertToArray(feed => new KeyValuePair<ObjectId, string>(feed.Id, feed.Name), _dashBoard.Feeds.Count);

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
            var deployment = _dashBoard.GetDeployment(deploymentId);

            var targetFeed = _dashBoard.Feeds.FirstOrDefault(item => item.Id.Equals(deployment.FeedId));

            //TODO: What else? Ignore silently? 
            if (targetFeed != null)
            {
                var task = taskFactory(deployment, targetFeed);
                task.Process();
            }
        }

        private object PostDeploy(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var version = new Version(parameters.version);

            var deployment = _dashBoard.GetDeployment(id);
            deployment.PackageVersion = version;

            SaveDeployment(deployment);

            ExecuteServiceControlAction(id, _taskBuilder.DeployServiceTask);

            return RedirectToDetails(id);
        }

        private object PostUpdate(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var version = new Version(parameters.version);

            var deployment = _dashBoard.GetDeployment(id);
            deployment.PackageVersion = version;

            SaveDeployment(deployment);

            ExecuteServiceControlAction(id, _taskBuilder.UpdateServiceTask);

            return RedirectToDetails(id);
        }

        private void SaveDeployment(Deployment deployment)
        {
            using (var tx = _deployments.BeginTransaction())
            {
                _deployments.Save(deployment, false);
                tx.Commit();
            }
            _versionCache.ForceRefresh();
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
            var deployment = _dashBoard.GetDeployment(id);
            return deployment;
        }
    }
}

using System;
using System.Collections.Generic;
using Codestellation.Emisstar;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Deployments;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager.Events;
using Codestellation.Galaxy.WebEnd.Models;
using Codestellation.Galaxy.WebEnd.Models.Deployment;
using Codestellation.Quarks.Collections;
using Codestellation.Quarks.IO;
using Nancy.ModelBinding;
using Nancy.Responses;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd
{
    public class DeploymentModule : CrudModule
    {
        private readonly FeedBoard _feedBoard;
        private readonly PackageVersionBoard _versionCache;

        private readonly DeploymentBoard _deploymentBoard;
        private readonly IPublisher _publisher;
        public const string Path = "deployment";

        public DeploymentModule(FeedBoard feedBoard, PackageVersionBoard versionCache, DeploymentBoard deploymentBoard, IPublisher publisher)
            : base(Path)
        {
            _feedBoard = feedBoard;
            _versionCache = versionCache;
            _deploymentBoard = deploymentBoard;
            _publisher = publisher;

            Post["/install/{id}", true] = (parameters, token) => ProcessRequest(() => PostInstall(parameters), token);
            Post["/start/{id}", true] = (parameters, token) => ProcessRequest(() => PostStart(parameters), token);
            Post["/stop/{id}", true] = (parameters, token) => ProcessRequest(() => PostStop(parameters), token);
            Post["/uninstall/{id}", true] = (parameters, token) => ProcessRequest(() => PostUninstall(parameters), token);
            Post["/deploy/{id}/{version}", true] = (parameters, token) => ProcessRequest(() => PostDeploy(parameters), token);

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
            get
            {
                return CrudOperations.GetList |
                    CrudOperations.GetCreate |
                    CrudOperations.PostCreate |
                    CrudOperations.GetEdit |
                    CrudOperations.PostEdit |
                    CrudOperations.PostDelete |
                    CrudOperations.GetDetails;
            }
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
            var anEvent = new DeleteDeploymentEvent(id);
            _publisher.Publish(anEvent);

            return RedirectToList();
        }

        protected override object GetDetails(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            Deployment deployment;

            if (_deploymentBoard.TryGetDeployment(id, out deployment))
            {
                deployment = _deploymentBoard.GetDeployment(id);
                var versions = _versionCache.GetPackageVersions(deployment.FeedId, deployment.PackageId);
                return View["details", new DeploymentModel(deployment, GetAvailableFeeds(), versions)];
            }
            else
            {
                return RedirectToList();
            }
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
            var message = new InstallServiceEvent(id);
            _publisher.Publish(message);

            return "ok";
        }

        private object PostStart(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            var message = new StartServiceEvent(id);
            _publisher.Publish(message);

            return "ok";
        }

        private object PostStop(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            var message = new StopServiceEvent(id);
            _publisher.Publish(message);

            return "ok";
        }

        private object PostUninstall(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            var message = new UninstallServiceEvent(id);
            _publisher.Publish(message);

            return "ok";
        }

        private object PostDeploy(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var version = new Version(parameters.version);

            var message = new DeployServiceEvent(id, version);

            _publisher.Publish(message);
            return "ok";
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
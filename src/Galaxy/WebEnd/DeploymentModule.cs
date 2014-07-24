using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nejdb;
using Nejdb.Bson;
using Nancy.Responses;
using Nancy.ModelBinding;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.WebEnd.Models;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager;
using System;
using Codestellation.Galaxy.ServiceManager.EventParams;
using System.Threading;
using System.IO;
using Nancy;

namespace Codestellation.Galaxy.WebEnd
{
    public class DeploymentModule : CrudModule
    {
        private readonly DashBoard _dashBoard;
        private readonly PackageVersionCache _versionCache;
        private readonly Collection _deployments;
        public const string Path = "deployment";

        public DeploymentModule(Repository repository, DashBoard dashBoard, PackageVersionCache versionCache)
            : base(Path)
        {
            _dashBoard = dashBoard;
            _versionCache = versionCache;
            _deployments = repository.GetCollection<Deployment>();

            Post["/install/{id}", true] = (parameters, token) => ProcessRequest(() => PostInstall(parameters), token);
            Post["/start/{id}", true] = (parameters, token) => ProcessRequest(() => PostStart(parameters), token);
            Post["/stop/{id}", true] = (parameters, token) => ProcessRequest(() => PostStop(parameters), token);
            Post["/uninstall/{id}", true] = (parameters, token) => ProcessRequest(() => PostUninstall(parameters), token);
            Post["/deploy/{id}", true] = (parameters, token) => ProcessRequest(() => PostDeploy(parameters), token);
            Post["/config/{id}", true] = (parameters, token) => ProcessRequest(() => PostConfig(parameters), token);
            Get["/config/{id}", true] = (parameters, token) => ProcessRequest(() => GetConfig(parameters), token);
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

            var versions = _versionCache.GetPackageVersions(deployment.FeedId, deployment.PackageId);

            return View["details", new DeploymentModel(deployment, GetAvailableFeeds(), versions)];
        }

        private KeyValuePair<ObjectId, string>[] GetAvailableFeeds()
        {
            var allFeeds = _dashBoard.Feeds.
                ConvertToArray(feed => new KeyValuePair<ObjectId, string>(feed.Id, feed.Name));
            return allFeeds;
        }

        private object PostInstall(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, Build.InstallServiceTask);         

            return RedirectToDetails(id);
        }

        private object PostStart(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, Build.StartServiceTask);         

            return RedirectToDetails(id);
        }

        private object PostStop(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, Build.StopServiceTask);         

            return RedirectToDetails(id);
        }

        private object PostUninstall(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);

            ExecuteServiceControlAction(id, Build.UninstallServiceTask);         

            return RedirectToDetails(id);
        }

        //TODO: three following methods are not eligible for web-module
        private void ExecuteServiceControlAction(ObjectId deploymentId, Func<Deployment, NugetFeed, DeploymentTask> taskFactory)
        {
            var deployment = _dashBoard.GetDeployment(deploymentId);

            var targetFeed = _dashBoard.Feeds.FirstOrDefault(item => item.Id.Equals(deployment.FeedId));            

            //TODO: What else? Ignore silently? 
            if (targetFeed != null)
            {
                var task = taskFactory(deployment, targetFeed);
                task.Process(OnDeploymentCompleted);
            }
        }
        
        private void OnDeploymentCompleted(DeploymentTaskCompletedEventArgs e)
        {
            Task.Factory.StartNew(() => UpdateDeploymentStatus(e), CancellationToken.None, TaskCreationOptions.None, SingleThreadScheduler.Instance);
        }

        private void UpdateDeploymentStatus(DeploymentTaskCompletedEventArgs e)
        {
            var deployment = _dashBoard.GetDeployment(e.Task.Deployment.Id);
            deployment.Status = e.Result.Details;

            SaveDeployment(deployment);
        }

        private object PostDeploy(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var deploymentModel = this.Bind<DeploymentModel>();

            var deployment = _dashBoard.GetDeployment(id);
            deployment.PackageVersion = deploymentModel.PackageVersion;

            SaveDeployment(deployment);

            ExecuteServiceControlAction(id, Build.DeployServiceTask);         

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

        private object PostConfig(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var content = ReceiveFile();
            var deployment = _dashBoard.GetDeployment(id);
            deployment.ConfigFileContent = content;
            SaveDeployment(deployment);

            return RedirectToDetails(id);
        }

        private string ReceiveFile()
        {
            var file = Request.Files.FirstOrDefault();
            var reader = new StreamReader(file.Value);
            var content = reader.ReadToEnd();
            return content;
        }

        private object GetConfig(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var deployment = _dashBoard.GetDeployment(id);

            var response = new Response();

            response.Headers.Add("Content-Disposition", "attachment; filename=config_preview.xml");
            response.ContentType = "text/xml";
            response.Contents = stream =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(deployment.ConfigFileContent);
                }
            };

            return response;
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

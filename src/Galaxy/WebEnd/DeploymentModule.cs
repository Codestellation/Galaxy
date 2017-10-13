using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.Emisstar;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Deployments;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager.Events;
using Codestellation.Galaxy.WebEnd.Controllers.Deployment;
using Codestellation.Galaxy.WebEnd.Models;
using Codestellation.Galaxy.WebEnd.Models.Deployment;
using Codestellation.Quarks.Collections;
using Codestellation.Quarks.IO;
using MediatR;
using Nancy.ModelBinding;
using Nancy.Responses;
using Nancy.Security;
using Nejdb;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd
{
    public class DeploymentModule : ModuleBase
    {
        private readonly IMediator _mediator;
        private readonly PackageBoard _packageBoard;

        private readonly DeploymentBoard _deploymentBoard;

        private readonly IPublisher _publisher;

        private readonly Collection _feedCollection;

        public const string Path = "deployment";

        public DeploymentModule(IMediator mediator, Repository repository, PackageBoard packageBoard, DeploymentBoard deploymentBoard, IPublisher publisher)
            : base(Path)
        {
            _feedCollection = repository.GetCollection<NugetFeed>();
            _mediator = mediator;
            _packageBoard = packageBoard;
            _deploymentBoard = deploymentBoard;
            _publisher = publisher;

            this.RequiresAuthentication();

            Get["/", true] = GetList;
            Get["/details/{id}", true] = GetDetails;

            Get["/create", true] = GetCreate;
            Post["/create", true] = (parameters, token) => ProcessRequest(() => PostCreate(parameters), token);

            Get["/edit/{id}", true] = (parameters, token) => ProcessRequest(() => GetEdit(parameters), token);
            Post["/edit/{id}", true] = (parameters, token) => ProcessRequest(() => PostEdit(parameters), token);

            Post["/delete/{id}", true] = (parameters, token) => ProcessRequest(() => PostDelete(parameters), token);

            Post["/install/{id}", true] = (parameters, token) => ProcessRequest(() => PostInstall(parameters), token);
            Post["/start/{id}", true] = (parameters, token) => ProcessRequest(() => PostStart(parameters), token);
            Post["/stop/{id}", true] = (parameters, token) => ProcessRequest(() => PostStop(parameters), token);
            Post["/uninstall/{id}", true] = (parameters, token) => ProcessRequest(() => PostUninstall(parameters), token);
            Post["/deploy/{id}/{version}", true] = (parameters, token) => ProcessRequest(() => PostDeploy(parameters), token);

            Get["/build-log/{id}", true] = (parameters, token) => ProcessRequest(() => GetBuildLogs(parameters), token);
            Get["/build-log/{id}/{filename}", true] = (parameters, token) => ProcessRequest(() => GetBuildLog(parameters), token);
        }

        private async Task<dynamic> GetList(dynamic parameters, CancellationToken token)
        {
            var request = new DeploymentListRequest();
            var response = await _mediator.Send(request, token).ConfigureAwait(false);
            return View["list", response.Model];
        }

        private async Task<dynamic> GetDetails(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);
            var request = new DeploymentDetailsRequest(id);
            var response = await _mediator.Send(request, token).ConfigureAwait(false);

            return response.Model == null
                ? RedirectToList()
                : View["details", response.Model];
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

        protected async Task<dynamic> GetCreate(dynamic parameters, CancellationToken token)
        {
            var request = new CreateDeploymentModelRequest();
            var response = await _mediator.Send(request, token).ConfigureAwait(false);

            return View["edit", response.Model];
        }

        protected object PostCreate(dynamic parameters)
        {
            var item = this.Bind<DeploymentEditModel>();
            var deployment = item.ToDeployment();

            _deploymentBoard.AddDeployment(deployment);

            return RedirectToList();
        }

        protected object GetEdit(dynamic parameters)
        {
            var deployment = GetDeployment(parameters);

            var deploymentEditModel = new DeploymentEditModel(deployment)
            {
                AllFeeds = GetAvailableFeeds()
            };
            return View["Edit", deploymentEditModel];
        }

        protected object PostEdit(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var model = this.Bind<DeploymentEditModel>();

            var deployment = _deploymentBoard.GetDeployment(id);

            model.Update(deployment);

            _deploymentBoard.SaveDeployment(deployment);

            return RedirectToDetails(id);
        }

        private object PostDelete(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var anEvent = new DeleteDeploymentEvent(id);
            _publisher.Publish(anEvent);

            return RedirectToList();
        }

        private KeyValuePair<ObjectId, string>[] GetAvailableFeeds()
        {
            var allFeeds =
                _feedCollection
                    .PerformQuery<NugetFeed>()
                    .ConvertToArray(feed => new KeyValuePair<ObjectId, string>(feed.Id, feed.Name));

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
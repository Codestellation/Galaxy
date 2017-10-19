using System;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Events;
using Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement;
using Codestellation.Galaxy.WebEnd.Models.DeploymentManangement;
using MediatR;
using Nancy.ModelBinding;
using Nancy.Responses;
using Nancy.Security;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd
{
    public class DeploymentModule : ModuleBase
    {
        private readonly IMediator _mediator;
        public const string Path = "deployment";

        public DeploymentModule(IMediator mediator)
            : base(Path)
        {
            _mediator = mediator;

            this.RequiresAuthentication();

            Get["/", true] = GetList;
            Get["/details/{id}", true] = GetDetails;

            Get["/create", true] = GetCreate;
            Post["/create", true] = PostCreate;

            Get["/edit/{id}", true] = GetEdit;
            Post["/edit/{id}", true] = PostEdit;

            Post["/delete/{id}", true] = PostDelete;

            Post["/install/{id}", true] = PostInstall;
            Post["/start/{id}", true] = PostStart;
            Post["/stop/{id}", true] = PostStop;
            Post["/uninstall/{id}", true] = PostUninstall;
            Post["/deploy/{id}/{version}", true] = PostDeploy;

            Post["backup/{id}", true] = RestoreBackup;
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

        private async Task<dynamic> GetCreate(dynamic parameters, CancellationToken token)
        {
            var request = new CreateDeploymentModelRequest();
            var response = await _mediator.Send(request, token).ConfigureAwait(false);
            return View["edit", response.Model];
        }

        private async Task<dynamic> PostCreate(dynamic parameters, CancellationToken token)
        {
            var model = this.Bind<DeploymentEditModel>();
            var request = new CreateDeploymentRequest(model);
            await _mediator.Send(request, token).ConfigureAwait(false);

            return RedirectToList();
        }

        private async Task<dynamic> GetEdit(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);
            var request = new EditDeploymentModelRequest(id);
            var response = await _mediator.Send(request, token).ConfigureAwait(false);

            return View["Edit", response.Model];
        }

        private async Task<dynamic> PostEdit(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);

            var model = this.Bind<DeploymentEditModel>();
            model.Id = id;
            var request = new EditDeploymentRequest(model);

            await _mediator.Send(request, token).ConfigureAwait(false);
            return RedirectToDetails(id);
        }

        private async Task<dynamic> PostDelete(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);
            var request = new DeploymentTaskRequest(id, Templates.Delete);
            await _mediator.Send(request, token).ConfigureAwait(false);
            return RedirectToList();
        }

        private async Task<dynamic> PostInstall(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);
            var request = new DeploymentTaskRequest(id, Templates.Install);
            return await SendCommand(request, token).ConfigureAwait(false);
        }

        private async Task<dynamic> PostUninstall(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);
            var request = new DeploymentTaskRequest(id, Templates.Uninstall);
            return await SendCommand(request, token).ConfigureAwait(false);
        }

        private async Task<dynamic> PostStart(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);
            var request = new DeploymentTaskRequest(id, Templates.Start);
            return await SendCommand(request, token).ConfigureAwait(false);
        }

        private async Task<dynamic> PostStop(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);
            var request = new DeploymentTaskRequest(id, Templates.Stop);
            return await SendCommand(request, token).ConfigureAwait(false);
        }

        private async Task<dynamic> PostDeploy(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);
            var version = new Version(parameters.version);
            var requestParameters = new { Version = version };
            var request = new DeploymentTaskRequest(id, Templates.Deploy, requestParameters);

            await _mediator.Send(request, token).ConfigureAwait(false);
            return "ok";
        }

        private async Task<dynamic> RestoreBackup(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);
            string name = Request.Query.name;
            var requestParameters = new { Name = name };
            var request = new DeploymentTaskRequest(id, Templates.Restore, requestParameters);
            await _mediator.Send(request, token).ConfigureAwait(false);
            return "ok";
        }

        private async Task<string> SendCommand(DeploymentTaskRequest request, CancellationToken token)
        {
            await _mediator.Send(request, token).ConfigureAwait(false);
            return "ok";
        }

        private static object RedirectToList()
        {
            return new RedirectResponse("/" + Path);
        }

        private static object RedirectToDetails(ObjectId id)
        {
            var detailsPath = $"/{Path}/details/{id}";
            return new RedirectResponse(detailsPath);
        }
    }
}
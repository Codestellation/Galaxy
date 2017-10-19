using System.Threading;
using System.Threading.Tasks;
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
        public const string TaskPath = "run-task";

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

            Post[$"/{TaskPath}", true] = RunTask;
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

        private async Task<object> RunTask(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(Request.Query.id);
            string taskName = Request.Query.task;
            var request = new DeploymentTaskRequest(id, taskName, Request.Query);
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
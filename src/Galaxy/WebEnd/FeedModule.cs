using System;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.Galaxy.WebEnd.Controllers.FeedManagement;
using Codestellation.Galaxy.WebEnd.Models;
using MediatR;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using Nancy.Security;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd
{
    public class FeedModule : ModuleBase
    {
        private readonly IMediator _mediator;
        public const string Path = "feed";

        public FeedModule(IMediator mediator)
            : base(Path)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.RequiresAuthentication();

            Get["/", true] = GetList;
            Get["/create"] = GetCreate;
            Post["/create", true] = PostCreate;
            Get["/edit/{id}", true] = GetEdit;
            Post["/edit/{id}", true] = PostEdit;
            Post["/delete/{id}", true] = PostDelete;
        }

        private async Task<dynamic> GetList(dynamic parameters, CancellationToken token)
        {
            var request = new FeedListRequest();
            var response = await _mediator.Send(request, token).ConfigureAwait(false);

            return View["list", response.Model];
        }

        private dynamic GetCreate(dynamic parameters)
        {
            return View["Edit", new FeedModel()];
        }

        private async Task<dynamic> PostCreate(dynamic parameters, CancellationToken token)
        {
            FeedModel model = this.Bind();
            var request = new SaveFeedRequest(model);
            await _mediator.Send(request, token).ConfigureAwait(false);
            return new RedirectResponse("/feed");
        }

        private async Task<dynamic> GetEdit(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);
            var request = new EditFeedModelRequest(id);
            var response = await _mediator.Send(request, token).ConfigureAwait(false);

            return View["Edit", response.Model];
        }

        private async Task<dynamic> PostEdit(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);
            FeedModel model = this.Bind();
            model.Id = id;
            var request = new EditFeedRequest(model);
            await _mediator.Send(request, token).ConfigureAwait(false);

            return new RedirectResponse("/feed");
        }

        private async Task<dynamic> PostDelete(dynamic parameters, CancellationToken token)
        {
            var id = new ObjectId(parameters.id);

            var request = new DeleteFeedRequest(id);
            var response = await _mediator.Send(request, token).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(response.Error))
            {
                return "Ok";
            }
            return new TextResponse(HttpStatusCode.BadRequest, response.Error);
        }
    }
}
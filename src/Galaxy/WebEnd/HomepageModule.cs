using System;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.Galaxy.WebEnd.Controllers.HomepageManagement;
using MediatR;
using Nancy.Security;

namespace Codestellation.Galaxy.WebEnd
{
    public class HomepageModule : ModuleBase
    {
        private readonly IMediator _mediator;

        public HomepageModule(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.RequiresAuthentication();

            Get["/", true] = OnRootAsync;
        }

        private async Task<dynamic> OnRootAsync(dynamic o, CancellationToken token)
        {
            var request = new HomepageModelRequest();
            var response = await _mediator.Send(request, token).ConfigureAwait(false);
            return View["Homepage", response.Model];
        }
    }
}
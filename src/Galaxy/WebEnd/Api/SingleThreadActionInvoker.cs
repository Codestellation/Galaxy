using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Codestellation.Galaxy.Infrastructure;

namespace Codestellation.Galaxy.WebEnd.Api
{
    public class SingleThreadActionInvoker : ApiControllerActionInvoker
    {
        public override Task<HttpResponseMessage> InvokeActionAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => base.InvokeActionAsync(actionContext, cancellationToken).Result, cancellationToken, TaskCreationOptions.None, SingleThreadScheduler.Instance);
        }
    }
}
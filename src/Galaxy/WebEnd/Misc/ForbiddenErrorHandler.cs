using Nancy;
using Nancy.ErrorHandling;
using Nancy.ViewEngines;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public class ForbiddenErrorHandler : IStatusCodeHandler
    {
        private readonly IViewRenderer _renderer;

        public ForbiddenErrorHandler(IViewRenderer renderer)
        {
            _renderer = renderer;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.NotFound;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return true;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            if (statusCode == HttpStatusCode.Forbidden)
            {
                context.Response = _renderer.RenderView(context, "Error403");
            }
        }
    }
}
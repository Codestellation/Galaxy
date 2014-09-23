using System.Collections.Generic;
using Microsoft.Owin;
using Nancy;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public static class NancyContextExtensions
    {
        private const string EnvironmentKey = "OWIN_REQUEST_ENVIRONMENT";

        public static OwinContext ToOwinContext(this NancyContext context)
        {
            var environment = (IDictionary<string, object>)context.Items[EnvironmentKey];
            var owinContext = new OwinContext(environment);
            return owinContext;
        }
    }
}
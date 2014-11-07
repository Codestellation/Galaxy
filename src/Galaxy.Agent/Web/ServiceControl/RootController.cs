using System.Web.Http;

namespace Codestellation.Galaxy.Agent.Web.ServiceControl
{
    public class RootController : ApiController
    {
        public Response<ServiceStatus> Get()
        {
            var status = new ServiceStatus();
            return Response.For(status);
        }
    }
}
using System.Web.Http;

namespace Codestellation.Galaxy.WebEnd.Api.ServiceControl
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
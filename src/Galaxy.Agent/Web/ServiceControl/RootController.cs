using System.Web.Http;
using Galaxy.Agent.Controllers;

namespace Galaxy.Agent
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
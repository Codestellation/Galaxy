using System.Web.Http;
using Codestellation.Quarks.Reflection;

namespace Galaxy.Agent
{
    public class RootController : ApiController
    {
        public string Get()
        {
            return AssemblyVersion.InformationalVersion;
        }
    }
}
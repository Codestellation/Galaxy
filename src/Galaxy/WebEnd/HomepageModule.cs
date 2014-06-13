using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.WebEnd.Models;
using Nancy.Security;

namespace Codestellation.Galaxy.WebEnd
{
    public class HomepageModule : ModuleBase
    {
        public HomepageModule(DashBoard dashBoard)
        {
            this.RequiresAuthentication();
            
            Get["/", true] = (parameters, token) => ProcessRequest(() => OnRoot(parameters), token);
        }
        private object OnRoot(dynamic parameters)
        {
            var model = new HomepageModel();
            return View["Homepage", model];
        }
    }
}
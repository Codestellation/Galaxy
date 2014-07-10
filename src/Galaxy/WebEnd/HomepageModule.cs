using Codestellation.Galaxy.WebEnd.Models;
using Nancy.Security;

namespace Codestellation.Galaxy.WebEnd
{
    public class HomepageModule : ModuleBase
    {
        public HomepageModule()
        {
            this.RequiresAuthentication();
            
            Get["/", true] = (parameters, token) => ProcessRequest(OnRoot, token);
        }
        private object OnRoot()
        {
            var model = new HomepageModel();
            return View["Homepage", model];
        }
    }
}
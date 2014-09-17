using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Galaxy.WebEnd.Models;
using Nancy.Security;

namespace Codestellation.Galaxy.WebEnd
{
    public class HomepageModule : ModuleBase
    {
        private readonly DashBoard _dashBoard;
        private readonly Notifier _notifier;

        public HomepageModule(DashBoard dashBoard, Notifier notifier)
        {
            _dashBoard = dashBoard;
            _notifier = notifier;
            this.RequiresAuthentication();
            
            Get["/", true] = (parameters, token) => ProcessRequest(OnRoot, token);
        }
        private object OnRoot()
        {
            var model = new HomepageModel(_dashBoard, _notifier);
            return View["Homepage", model];
        }
    }
}
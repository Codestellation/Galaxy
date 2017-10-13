using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Galaxy.WebEnd.Models;
using Nancy.Security;

namespace Codestellation.Galaxy.WebEnd
{
    public class HomepageModule : ModuleBase
    {
        private readonly NotificationBoard _notificationBoard;

        public HomepageModule(NotificationBoard notificationBoard)
        {
            _notificationBoard = notificationBoard;
            this.RequiresAuthentication();

            Get["/", true] = (parameters, token) => ProcessRequest(OnRoot, token);
        }

        private object OnRoot()
        {
            var model = new HomepageModel(_notificationBoard);
            return View["Homepage", model];
        }
    }
}
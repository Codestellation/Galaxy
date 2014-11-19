using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Galaxy.WebEnd.Models;
using Nancy.Security;

namespace Codestellation.Galaxy.WebEnd
{
    public class HomepageModule : ModuleBase
    {
        private readonly FeedBoard _feedBoard;
        private readonly NotificationBoard _notificationBoard;

        public HomepageModule(FeedBoard feedBoard, NotificationBoard notificationBoard)
        {
            _feedBoard = feedBoard;
            _notificationBoard = notificationBoard;
            this.RequiresAuthentication();
            
            Get["/", true] = (parameters, token) => ProcessRequest(OnRoot, token);
        }
        private object OnRoot()
        {
            var model = new HomepageModel(_feedBoard, _notificationBoard);
            return View["Homepage", model];
        }
    }
}
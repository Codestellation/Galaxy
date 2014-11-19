using System.Collections.Generic;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Notifications;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class HomepageModel
    {
        public readonly IList<Notification> Errors;
        public readonly IList<Notification> Events;

        public HomepageModel(FeedBoard feedBoard, NotificationBoard notificationBoard)
        {
            Errors = notificationBoard.GetNotifications(x => x.Severity == Severity.Error, 10);
            Events = notificationBoard.GetNotifications(x => x.Severity < Severity.Error, 10);
        }
    }
}
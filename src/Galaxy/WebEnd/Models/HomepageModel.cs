using System.Collections.Generic;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Notifications;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class HomepageModel
    {
        public readonly IList<Notification> Errors;
        public readonly IList<Notification> Events;

        public HomepageModel(DashBoard dashBoard, Notifier notifier)
        {
            Errors = notifier.GetNotifications(x => x.Severity == Severity.Error, 10);
            Events = notifier.GetNotifications(x => x.Severity < Severity.Error, 10);
        }
    }
}
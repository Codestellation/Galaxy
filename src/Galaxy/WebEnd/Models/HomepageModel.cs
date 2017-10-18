using System;
using System.Collections.Generic;
using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Quarks.DateAndTime;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class HomepageModel
    {
        public readonly IList<Notification> Errors;
        public readonly IList<Notification> Events;

        public bool HasErrors => Errors.Count > 0;
        public bool HasEvents => Events.Count > 0;

        public bool NoNotifications => Errors.Count + Events.Count == 0;

        public HomepageModel(NotificationBoard notificationBoard)
        {
            DateTime now = Clock.UtcNow;

            Errors = notificationBoard.GetNotifications(x => x.Severity == Severity.Error && x.CreatedAt >= now.AddDays(-30), 10);
            Events = notificationBoard.GetNotifications(x => x.Severity < Severity.Error && x.CreatedAt >= now.AddDays(-30), 10);
        }
    }
}
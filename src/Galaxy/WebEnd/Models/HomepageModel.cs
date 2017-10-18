using System.Collections.Generic;
using System.Linq;
using Codestellation.Galaxy.Domain.Notifications;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class HomepageModel
    {
        public readonly IList<Notification> Errors;
        public readonly IList<Notification> Events;

        public bool HasErrors => Errors.Count > 0;
        public bool HasEvents => Events.Count > 0;

        public bool NoNotifications => Errors.Count + Events.Count == 0;

        public HomepageModel(List<Notification> notifications)
        {
            Errors = notifications.Where(x => x.Severity == Severity.Error).Take(10).ToList();
            Events = notifications.Where(x => x.Severity < Severity.Error).Take(10).ToList();
        }
    }
}
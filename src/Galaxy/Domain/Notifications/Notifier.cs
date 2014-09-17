using System;
using System.Collections.Generic;

namespace Codestellation.Galaxy.Domain.Notifications
{
    public class Notifier
    {
        private readonly List<Notification> _notifications;

        public Notifier()
        {
            _notifications = new List<Notification>();
        }

        public void Notify(Notification notification)
        {
            lock (notification)
            {
                _notifications.Add(notification);
            }
        }

        public IList<Notification> GetNotifications(Predicate<Notification> predicate, int count)
        {
            var result = new List<Notification>(count);

            lock (_notifications)
            {
                for (int index = _notifications.Count - 1; index >= 0; index--)
                {
                    var notification = _notifications[index];
                    if (predicate(notification))
                    {
                        continue;
                    }

                    result.Add(notification);

                    if (result.Count == count)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}
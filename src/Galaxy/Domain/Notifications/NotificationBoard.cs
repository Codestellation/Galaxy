using System;
using System.Collections.Generic;
using Codestellation.Galaxy.Infrastructure;
using Nejdb;

namespace Codestellation.Galaxy.Domain.Notifications
{
    public class NotificationBoard
    {
        private readonly List<Notification> _notifications;
        private readonly Collection _notificationCollection;

        public NotificationBoard(Repository repository)
        {
            _notifications = new List<Notification>();
            _notificationCollection = repository.GetCollection<Notification>();
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            lock (_notifications)
            {
                _notifications.AddRange(_notificationCollection.PerformQuery<Notification>());
                _notifications.Sort((x, y) => x.CreatedAt.CompareTo(y.CreatedAt));
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
                        result.Add(notification);
                    }

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
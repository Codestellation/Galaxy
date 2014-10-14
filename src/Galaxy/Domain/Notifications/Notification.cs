using System;
using Codestellation.Quarks.DateAndTime;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain.Notifications
{
    public class Notification
    {
        public DateTime CreatedAt { get; private set; }
        public ObjectId? DeploymentId { get; private set; }
        public string Message { get; private set; }
        public Severity Severity { get; private set; }

        public string Url;

        public Notification(ObjectId? deploymentId, string message, Severity severity)
        {
            DeploymentId = deploymentId;
            Message = message;
            Severity = severity;
            CreatedAt = Clock.UtcNow;
        }

        public static Notification Info(ObjectId? deploymentId, string message)
        {
            return new Notification(deploymentId,message, Severity.Info);
        }

        public static Notification Warning(ObjectId? deploymentId, string message)
        {
            return new Notification(deploymentId, message, Severity.Warning);
        }

        public static Notification Error(ObjectId? deploymentId, string message)
        {
            return new Notification(deploymentId, message, Severity.Error);
        }
    }
}
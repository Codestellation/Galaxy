using System;
using Codestellation.Quarks.DateAndTime;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain.Notifications
{
    public class Notification
    {
        public readonly DateTime CreatedAt;
        public readonly ObjectId? DeploymentId;
        public readonly string Message;
        public readonly Severity Severity;

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
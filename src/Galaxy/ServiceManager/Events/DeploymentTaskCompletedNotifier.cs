using Codestellation.Emisstar;
using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.WebEnd;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeploymentTaskCompletedNotifier : IHandler<DeploymentTaskCompletedEvent>
    {
        private readonly Notifier _notifier;

        public DeploymentTaskCompletedNotifier(Notifier notifier)
        {
            _notifier = notifier;
        }

        public void Handle(DeploymentTaskCompletedEvent message)
        {
            var operationResult = message.Result;
            var url = string.Format("/{0}/details/{1}", @DeploymentModule.Path, message.Task.DeploymentId);
            var severity = operationResult.ResultCode == ResultCode.Succeed ? Severity.Info : Severity.Error;
            var notification = new Notification(message.Task.DeploymentId, operationResult.Details, severity)
            {
                Url = url
            };
            _notifier.Notify(notification);
        }
    }
}
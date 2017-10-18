using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.WebEnd;
using MediatR;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeploymentTaskCompletedNotifier : IRequestHandler<DeploymentTaskCompletedEvent>
    {
        private readonly Repository _repository;

        public DeploymentTaskCompletedNotifier(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(DeploymentTaskCompletedEvent message)
        {
            var operationResult = message.Result;
            var url = $"/{@DeploymentModule.Path}/details/{message.Task.DeploymentId}";
            var severity = operationResult.ResultCode == ResultCode.Succeed ? Severity.Info : Severity.Error;
            var notification = new Notification(message.Task.DeploymentId, operationResult.Details, severity)
            {
                Url = url
            };
            using (var tx = _repository.Notifications.BeginTransaction())
            {
                _repository.Notifications.Save(notification, false);
                tx.Commit();
            }
        }
    }
}
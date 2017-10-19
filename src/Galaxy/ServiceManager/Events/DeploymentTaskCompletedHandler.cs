using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.WebEnd;
using MediatR;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeploymentTaskCompletedHandler : IRequestHandler<DeploymentTaskCompletedEvent>
    {
        private readonly Repository _repository;

        public DeploymentTaskCompletedHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(DeploymentTaskCompletedEvent message)
        {
            UpdateDeployment(message);
            UpdateNotification(message);
        }

        private void UpdateDeployment(DeploymentTaskCompletedEvent message)
        {
            var deployments = _repository.Deployments;
            using (var tx = deployments.BeginTransaction())
            {
                var deployment = deployments.Load<Deployment>(message.Task.DeploymentId);
                if (deployment == null)
                {
                    return;
                }

                deployment.Status = message.Result.Details;
                deployment.PackageVersion = message.Task.Context.InstalledPackageVersion ?? deployment.PackageVersion;
                deployments.Save(deployment, false);
                tx.Commit();
            }
        }

        private void UpdateNotification(DeploymentTaskCompletedEvent message)
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
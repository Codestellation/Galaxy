using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
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
            var deployments = _repository.Deployments;
            using (var tx = deployments.BeginTransaction())
            {
                var deployment = deployments.Load<Deployment>(message.Task.DeploymentId);
                if (deployment == null)
                {
                    return;
                }

                deployment.Status = message.Result.Details;

                deployments.Save(deployment, false);
                tx.Commit();
            }
        }
    }
}
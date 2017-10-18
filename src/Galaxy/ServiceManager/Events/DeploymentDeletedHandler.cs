using System;
using Codestellation.Galaxy.Infrastructure;
using MediatR;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeploymentDeletedHandler : IRequestHandler<DeploymentDeletedEvent>
    {
        private readonly Repository _repository;

        public DeploymentDeletedHandler(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void Handle(DeploymentDeletedEvent message)
        {
            using (var tx = _repository.Deployments.BeginTransaction())
            {
                _repository.Deployments.Delete(message.DeploymentId);
                tx.Commit();
            }
        }
    }
}
using System;
using Codestellation.Galaxy.Infrastructure;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class EditDeploymentHandler : IRequestHandler<EditDeploymentRequest>
    {
        private readonly Repository _repository;

        public EditDeploymentHandler(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public void Handle(EditDeploymentRequest message)
        {
            using (var tx = _repository.Deployments.BeginTransaction())
            {
                var deployment = _repository.Deployments.Load<Domain.Deployment>(message.Model.Id);

                message.Model.Update(deployment);
                _repository.Deployments.Save(deployment, false);
                tx.Commit();
            }
        }
    }
}
using System;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using MediatR;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class ConfigSampleReceivedHandler : IRequestHandler<SetConfigSampleRequest>
    {
        private readonly Repository _repository;

        public ConfigSampleReceivedHandler(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void Handle(SetConfigSampleRequest message)
        {
            var deployments = _repository.Deployments;

            using (var tx = deployments.BeginTransaction())
            {
                var deployment = deployments.Load<Deployment>(message.DeploymentId);
                deployment.ConfigSample = message.Sample;
                deployments.Save(deployment, false);
                tx.Commit();
            }
        }
    }
}
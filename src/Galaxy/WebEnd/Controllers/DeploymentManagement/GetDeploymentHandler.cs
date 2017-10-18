using System;
using Codestellation.Galaxy.Infrastructure;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class GetDeploymentHandler : IRequestHandler<GetDeploymentRequest, GetDeploymentResponse>
    {
        private readonly Repository _repository;

        public GetDeploymentHandler(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public GetDeploymentResponse Handle(GetDeploymentRequest message)
        {
            var deployment = _repository.Deployments.Load<Domain.Deployment>(message.Id);
            return new GetDeploymentResponse(deployment);
        }
    }
}
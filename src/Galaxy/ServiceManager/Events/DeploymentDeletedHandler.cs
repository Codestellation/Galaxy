using Codestellation.Emisstar;
using Codestellation.Galaxy.Domain.Deployments;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeploymentDeletedHandler : IHandler<DeploymentDeletedEvent>
    {
        private readonly DeploymentBoard _board;

        public DeploymentDeletedHandler(DeploymentBoard board)
        {
            _board = board;
        }

        public void Handle(DeploymentDeletedEvent message)
        {
            _board.RemoveDeployment(message.DeploymentId);
        }
    }
}
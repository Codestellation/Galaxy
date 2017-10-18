using Codestellation.Galaxy.ServiceManager.Events;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class PublishDeploymentDeletedEvent : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            context
                .Mediator
                .Send(new DeploymentDeletedEvent(context.DeploymentId)).Wait();
        }
    }
}
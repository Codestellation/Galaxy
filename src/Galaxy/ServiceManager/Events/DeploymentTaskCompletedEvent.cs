using Codestellation.Galaxy.ServiceManager.Operations;
using MediatR;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeploymentTaskCompletedEvent : IRequest
    {
        public readonly OperationResult Result;
        public readonly DeploymentTask Task;

        public DeploymentTaskCompletedEvent(DeploymentTask task, OperationResult result)
        {
            Result = result;
            Task = task;
        }

        public override string ToString() => $"{Task.Name}:{Result}";
    }
}
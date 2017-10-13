using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeploymentTaskCompletedEvent : IMainRequest
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
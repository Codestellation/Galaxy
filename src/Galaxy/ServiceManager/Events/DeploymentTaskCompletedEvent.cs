using Codestellation.Galaxy.Infrastructure.Emisstar;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    [Synchronized]
    public class DeploymentTaskCompletedEvent 
    {
        public readonly OperationResult Result;
        public readonly DeploymentTask Task;

        public DeploymentTaskCompletedEvent(DeploymentTask task, OperationResult result)
        {
            Result = result;
            Task = task;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Task.Name, Result);
        }
    }
}

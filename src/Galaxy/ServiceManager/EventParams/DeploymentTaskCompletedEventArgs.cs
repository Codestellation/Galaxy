using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager.EventParams
{
    public class DeploymentTaskCompletedEventArgs 
    {
        private readonly OperationResult _result;
        private readonly DeploymentTask _deploymentTask;

        public DeploymentTask Task
        {
            get { return _deploymentTask; }
        }

        public OperationResult Result
        {
            get { return _result; }
        }

        public DeploymentTaskCompletedEventArgs(DeploymentTask deploymentTask, OperationResult result)
        {
            _result = result;
            _deploymentTask = deploymentTask;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Task.Name, Result);
        }
    }
}

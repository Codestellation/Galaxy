using System;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager.EventParams
{
    public class DeploymentTaskCompletedEventArgs: EventArgs
    {
        private readonly OperationResult _result;
        private readonly DeploymentTask _deploymentTask;

        public DeploymentTask Task
        {
            get { return _deploymentTask; }
        } 

        public DeploymentTaskCompletedEventArgs(DeploymentTask deploymentTask, OperationResult result)
        {
            _result = result;
            _deploymentTask = deploymentTask;
        }

        public OperationResult Result
        {
            get { return _result; }
        }
    }
}

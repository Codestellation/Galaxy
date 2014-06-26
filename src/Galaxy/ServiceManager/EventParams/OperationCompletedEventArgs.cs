using Codestellation.Galaxy.Domain;
using System;

namespace Codestellation.Galaxy.ServiceManager.EventParams
{
    public class DeploymentTaskCompletedEventArgs: EventArgs
    {
        private readonly string _operation;

        private readonly OperationResult _result;

        private readonly string _details;
        private readonly DeploymentTask _deploymentTask;

        public DeploymentTask Task
        {
            get { return _deploymentTask; }
        } 


        public DeploymentTaskCompletedEventArgs(DeploymentTask deploymentTask, string operation, OperationResult result, string details)
        {
            _operation = operation;
            _result = result;
            _details = details;
            _deploymentTask = deploymentTask;
        }
        public string Operation
        {
            get { return _operation; }
        }
        public OperationResult Result
        {
            get { return _result; }
        }
        public string Details
        {
            get { return _details; }
        }
    }
}

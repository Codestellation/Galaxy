using System;
using Codestellation.Galaxy.ServiceManager.EventParams;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.ServiceManager.Helpers;
using System.Threading.Tasks;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeploymentProcessor
    {
        private Task _task;

        private void ProcessInternal(DeploymentTask deploymentTask, Action<DeploymentTaskCompletedEventArgs> callback)
        {
            var results = new OperationResult[deploymentTask.Operations.Count];

            var failureDetected = false;

            for (int index = 0; index < deploymentTask.Operations.Count; index++)
            {
                var operation = deploymentTask.Operations[index];

                string operationName = operation.GetType().Name;

                if (failureDetected)
                {
                    results[index] = new OperationResult(operationName, ResultCode.NotRan);
                    continue;
                }

                try
                {
                    operation.Execute();
                    results[index] = new OperationResult(operationName, ResultCode.Succeed);
                }
                catch (Exception ex)
                {
                    results[index] = new OperationResult(operationName, ResultCode.Failed, ex.Message);
                    failureDetected = true;
                }
            }

            var deploymentResult = ResultsDescriberHelper.AggregateResults(deploymentTask, results);

            var args = new DeploymentTaskCompletedEventArgs(deploymentTask, deploymentResult);
            
            callback.Invoke(args);
        }

        public void Process(DeploymentTask deploymentTask, Action<DeploymentTaskCompletedEventArgs> callback)
        {
            //All exceptions are catched. No need to prevent task finalizers faults.
            _task =  Task.Factory.StartNew(() => ProcessInternal(deploymentTask, callback));
        }

        public void Wait()
        {
            _task.Wait();
        }
    }
}

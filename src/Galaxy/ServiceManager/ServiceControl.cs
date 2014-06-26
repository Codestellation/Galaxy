using System;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.EventParams;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeploymentProcessor
    {
        public DeploymentProcessor()
        {
        }

        public void Process(DeploymentTask deploymentTask, EventHandler<DeploymentTaskCompletedEventArgs> callback)
        {
            Task tsk = new Task(
                () =>
                {
                    Queue<ServiceOperation> localQueue = new Queue<ServiceOperation>(deploymentTask.Operations);

                    OperationResult[] results = new OperationResult[localQueue.Count];
                    int index = 0;
                    while (localQueue.Count > 0)
                    {
                        var operation = localQueue.Dequeue();
                        operation.Execute();
                        results[index++] = operation.Result;
                    }

                    var success = results.FirstOrDefault(item => item == OperationResult.OR_FAIL) == default(OperationResult);

                    callback.Invoke(this, 
                        new DeploymentTaskCompletedEventArgs(
                            deploymentTask, "all", success ? OperationResult.OR_OK : OperationResult.OR_FAIL, ""));
                });
            tsk.Start();
        }

    }
}

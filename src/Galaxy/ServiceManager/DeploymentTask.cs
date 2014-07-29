using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Codestellation.Galaxy.Domain;
using System.Collections.Generic;
using Codestellation.Galaxy.ServiceManager.EventParams;
using Codestellation.Galaxy.ServiceManager.Operations;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeploymentTask
    {
        private Task _task;
        
        private readonly List<IOperation> _operations; 

        private readonly string _name;
        private readonly ObjectId _deploymentId;

        private readonly StringWriter _buildLog;


        public IReadOnlyList<IOperation> Operations
        {
            get { return _operations; }
        }

        public DeploymentTask Add(IOperation operation)
        {
            _operations.Add(operation);
            return this;
        }

        public ObjectId DeploymentId
        {
            get { return _deploymentId; }
        }

        public string Name
        {
            get { return _name; }
        }

        private DeploymentTask()
        {
            _operations =new List<IOperation>();
            var stringBuilder = new StringBuilder(4000);
            _buildLog = new StringWriter(stringBuilder);
        }

        public DeploymentTask(string name, ObjectId deploymentId) : this()
        {
            _name = name;
            _deploymentId = deploymentId;
        }

        public DeploymentTask(string name, ObjectId deploymentId, IEnumerable<IOperation> operations) :
            this(name, deploymentId)
        {
            _operations.AddRange(operations);
        }


        public void Process(Action<DeploymentTaskCompletedEventArgs> callback)
        {
            //All exceptions are catched. No need to prevent task finalizers faults.
            _task = Task.Factory.StartNew(() => ProcessInternal(callback));
        }

        public void Wait()
        {
            _task.Wait();
        }

        private void ProcessInternal(Action<DeploymentTaskCompletedEventArgs> callback)
        {
            var results = new OperationResult[Operations.Count];

            var failureDetected = false;

            for (int index = 0; index < Operations.Count; index++)
            {
                var operation = Operations[index];

                string operationName = operation.GetType().Name;

                if (failureDetected)
                {
                    results[index] = new OperationResult(operationName, ResultCode.NotRan);
                    continue;
                }

                try
                {
                    operation.Execute(_buildLog);
                    results[index] = new OperationResult(operationName, ResultCode.Succeed);
                }
                catch (Exception ex)
                {
                    results[index] = new OperationResult(operationName, ResultCode.Failed, ex.Message);
                    failureDetected = true;
                }
            }

            var deploymentResult = new OperationResult(Name, results);

            var args = new DeploymentTaskCompletedEventArgs(this, deploymentResult);

            callback.Invoke(args);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}

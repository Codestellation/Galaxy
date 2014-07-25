using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codestellation.Galaxy.Domain;
using System.Collections.Generic;
using Codestellation.Galaxy.ServiceManager.EventParams;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeploymentTask
    {
        private Task _task;
        
        private readonly List<OperationBase> _operations; 

        private readonly string _name;
        private readonly Deployment _deployment;
        private readonly NugetFeed _feed;
        private readonly string _basePath;
        private StringBuilder _buildLog;

        public NugetFeed Feed
        {
            get { return _feed; }
        }

        public string BasePath
        {
            get { return _basePath; }
        } 

        public IReadOnlyList<OperationBase> Operations
        {
            get { return _operations; }
        }

        public void Add(OperationBase operation)
        {
            _operations.Add(operation);
        }

        public Deployment Deployment
        {
            get { return _deployment; }
        }

        public string Name
        {
            get { return _name; }
        }

        private DeploymentTask()
        {
            _operations =new List<OperationBase>();
            _buildLog = new StringBuilder(4000);
        }

        public DeploymentTask(string name, Deployment deployment, NugetFeed feed, string basePath) : this()
        {
            _name = name;
            _deployment = deployment;
            _feed = feed;
            _basePath = basePath;
        }

        public DeploymentTask(string name, Deployment deployment, NugetFeed feed, string basePath, IEnumerable<OperationBase> operations) :
            this(name, deployment, feed, basePath)
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

using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Codestellation.Emisstar;
using Codestellation.Galaxy.ServiceManager.Events;
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
        private readonly Stream _logStream;
        private readonly IPublisher _publisher;

        private OperationResult[] _operationResults;
        private DeploymentTaskContext _context;

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

        public DeploymentTask(string name, ObjectId deploymentId, Stream logStream, IPublisher publisher)
        {
            _operations = new List<IOperation>();

            _name = name;
            _deploymentId = deploymentId;
            _logStream = logStream;
            _publisher = publisher;
            var streamWriter = new StreamWriter(logStream);
            _context = new DeploymentTaskContext(streamWriter);
            
        }

        public void Process()
        {
            //All exceptions are catched. No need to prevent task finalizers faults.
            _task = Task.Factory.StartNew(ProcessInternal);
        }

        public void Wait()
        {
            _task.Wait();
        }

        private void ProcessInternal()
        {
            try
            {
                _context.BuildLog.WriteLine("Task '{0}' started", _name);
                ExecuteOperations();
                _context.BuildLog.WriteLine("Task '{0}' finished", _name);
            }
            finally
            {
                _context.BuildLog.Dispose();
            }

            var deploymentResult = new OperationResult(Name, _operationResults);
            var anEvent =  new DeploymentTaskCompletedEvent(this, deploymentResult);
            _publisher.Publish(anEvent);
        }

        private void ExecuteOperations()
        {
            _operationResults = new OperationResult[Operations.Count];
            var failureDetected = false;
            for (var index = 0; index < Operations.Count; index++)
            {
                var operation = Operations[index];

                string operationName = operation.GetType().Name;

                var operationIndex = index + 1;

                _context.BuildLog.WriteLine("Operation '{0}' started. ({1}/{2})", operationName, operationIndex, Operations.Count);

                var operationResult = _operationResults[index] = 
                    failureDetected 
                    ? new OperationResult(operationName, ResultCode.NotRan) //do not execute operation if any previous failed
                    : Execute(operation);

                WriteResult(operationResult, operationIndex);

                failureDetected = operationResult.ResultCode != ResultCode.Succeed;
            }
        }

        private OperationResult Execute(IOperation operation)
        {
            var operationName = operation.GetType().Name;

            try
            {
                
                operation.Execute(_context);
                return new OperationResult(operationName, ResultCode.Succeed);

            }
            catch (Exception ex)
            {
                _context.BuildLog.WriteLine(ex.ToString());
                return new OperationResult(operationName, ResultCode.Failed, ex.Message);
            }
        }

        private void WriteResult(OperationResult operationResult, int operationIndex)
        {
            var operationName = operationResult.OperationName;

            string result = DefineResult(operationResult);

            _context.BuildLog.WriteLine("Operation '{0}' {1}. ({2}/{3})", operationName, result, operationIndex, Operations.Count);
        }

        private string DefineResult(OperationResult operationResult)
        {
            switch (operationResult.ResultCode)
            {
                case ResultCode.Succeed:
                    return "succeed";
                case ResultCode.Failed:
                    return "failed";
                case ResultCode.NotRan:
                    return "skipped";
            }
            throw new InvalidOperationException();
        }

        public override string ToString()
        {
            return _name;
        }
    }
}

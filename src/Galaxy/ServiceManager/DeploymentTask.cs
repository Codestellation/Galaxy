﻿using System;
using System.IO;
using System.Threading.Tasks;
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
        private readonly Stream _logStream;

        private StreamWriter _buildLog;
        private OperationResult[] _operationResults;


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

        public DeploymentTask(string name, ObjectId deploymentId, Stream logStream)
        {
            _operations = new List<IOperation>();

            _name = name;
            _deploymentId = deploymentId;
            _logStream = logStream;
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
            using (_buildLog = new StreamWriter(_logStream))
            {
                _buildLog.WriteLine("Task '{0}' started", _name);

                ExecuteOperations();

                _buildLog.WriteLine("Task '{0}' finished", _name);
            }

            var deploymentResult = new OperationResult(Name, _operationResults);

            var args = new DeploymentTaskCompletedEventArgs(this, deploymentResult);

            callback.Invoke(args);
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

                _buildLog.WriteLine("Operation '{0}' started. ({1}/{2})", operationName, operationIndex, Operations.Count);

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
                operation.Execute(_buildLog);
                return new OperationResult(operationName, ResultCode.Succeed);

            }
            catch (Exception ex)
            {
                _buildLog.WriteLine(ex.ToString());
                return new OperationResult(operationName, ResultCode.Failed, ex.Message);
            }
        }

        private void WriteResult(OperationResult operationResult, int operationIndex)
        {
            var operationName = operationResult.OperationName;

            string result = DefineResult(operationResult);

            _buildLog.WriteLine("Operation '{0}' {1}. ({2}/{3})", operationName, result, operationIndex, Operations.Count);
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

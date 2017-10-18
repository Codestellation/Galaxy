using System;
using System.Collections.Generic;
using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Galaxy.ServiceManager.Events;
using Codestellation.Galaxy.ServiceManager.Operations;
using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeploymentTask
    {
        public readonly DeploymentTaskContext Context;

        private readonly List<IOperation> _operations;

        private OperationResult[] _operationResults;

        public IReadOnlyList<IOperation> Operations => _operations;

        public DeploymentTask Add(IOperation operation)
        {
            _operations.Add(operation);
            return this;
        }

        public ObjectId DeploymentId => Context.DeploymentId;

        public string Name => Context.GetValue<string>(DeploymentTaskContext.TaskName);

        public IMediator Mediator => Context.Mediator;

        public DeploymentTask(DeploymentTaskContext context)
        {
            _operations = new List<IOperation>();
            Context = context;
        }

        public void Process()
        {
            try
            {
                Context.BuildLog.WriteLine("Task '{0}' started", Name);
                ExecuteOperations();
                Context.BuildLog.WriteLine("Task '{0}' finished", Name);
            }
            finally
            {
                Context.BuildLog.Flush();
                Context.BuildLog.Dispose();
            }

            var deploymentResult = new OperationResult(Name, _operationResults);
            var anEvent = new DeploymentTaskCompletedEvent(this, deploymentResult);
            Mediator.Send(anEvent).Wait();
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

                Context.BuildLog.WriteLine("Operation '{0}' started. ({1}/{2})", operationName, operationIndex, Operations.Count);

                var operationResult = _operationResults[index] =
                    failureDetected
                        ? new OperationResult(operationName, ResultCode.NotRan) //do not execute operation if any previous failed
                        : Execute(operation);

                WriteResult(operationResult, operationIndex);

                failureDetected = operationResult.ResultCode != ResultCode.Succeed;
                PublishProgress(index, operationResult);
            }
        }

        private OperationResult Execute(IOperation operation)
        {
            var operationName = operation.GetType().Name;
            OperationResult result = null;
            try
            {
                operation.Execute(Context);
                result = new OperationResult(operationName, ResultCode.Succeed);
            }
            catch (Exception ex)
            {
                Context.BuildLog.WriteLine(ex.ToString());
                result = new OperationResult(operationName, ResultCode.Failed, ex.Message);
            }
            return result;
        }

        private void WriteResult(OperationResult operationResult, int operationIndex)
        {
            var operationName = operationResult.OperationName;

            string result = DefineResult(operationResult);

            Context.BuildLog.WriteLine("Operation '{0}' {1}. ({2}/{3})", operationName, result, operationIndex, Operations.Count);
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

        private void PublishProgress(int index, OperationResult result)
        {
            var notification = new OperationProgressNotification(++index, _operations.Count, result);
            Mediator.Publish(notification);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
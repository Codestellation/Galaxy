using Codestellation.Galaxy.Domain;
using System;

namespace Codestellation.Galaxy.ServiceManager.EventParams
{
    public class OperationCompletedEventArgs: EventArgs
    {
        private readonly string _operation;

        private readonly OperationResult _result;

        private readonly string _details;

        private readonly Deployment _deployment;

        private readonly NugetFeed _feed;

        public OperationCompletedEventArgs(Deployment deployment, NugetFeed feed, string operation, OperationResult result, string details)
        {
            _operation = operation;
            _result = result;
            _details = details;
            _feed = feed;
            _deployment = deployment;
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
        public Deployment Deployment
        {
            get { return _deployment; }
        }
    }
}

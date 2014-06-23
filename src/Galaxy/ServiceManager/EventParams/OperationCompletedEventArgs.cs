using Codestellation.Galaxy.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestellation.Galaxy.ServiceManager.EventParams
{
    public class OperationCompletedEventArgs: EventArgs
    {
        readonly string _operation;

        readonly OperationResult _result;

        readonly string _details;

        readonly ServiceApp _serviceApp;

        readonly NugetFeed _feed;

        public OperationCompletedEventArgs(ServiceApp serviceApp, NugetFeed feed, 
                                           string operation, OperationResult result, string details)
        {
            this._operation = operation;
            this._result = result;
            this._details = details;
            this._feed = feed;
            this._serviceApp = serviceApp;
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
        public ServiceApp ServiceApp
        {
            get { return _serviceApp; }
        }
    }
}

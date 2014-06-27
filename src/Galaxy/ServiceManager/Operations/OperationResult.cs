using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public enum OperationResultType
    {
        OR_DEFAULT,
        OR_OK,
        OR_FAIL
    }

    public class OperationResult
    {
        readonly string _operationName;

        public string OperationName
        {
            get { return _operationName; }
        } 


        readonly OperationResult[] _children = null;

        readonly OperationResultType _resultType;

        public OperationResultType ResultType
        {
            get { return _resultType; }
        }

        readonly string _details;

        public string Details
        {
            get { return _details; }
        } 

        public IEnumerable<OperationResult> GetFailedChildren()
        {
            return (_children ?? new OperationResult[0]).Where(item => item._resultType == OperationResultType.OR_FAIL);
        }

        public OperationResult(string operationName, OperationResultType resultType, string details)
        {
            _resultType = resultType;
            _details = details;
            _operationName = operationName;
        }

        public OperationResult(string operationName, OperationResultType resultType, string details, OperationResult[] children) :
            this(operationName, resultType, details)
        {
            _children = children;
        }
    }
}

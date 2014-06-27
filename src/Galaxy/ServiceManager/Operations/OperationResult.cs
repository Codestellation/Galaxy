namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class OperationResult
    {
        private readonly OperationResult[] _children;
        private readonly string _details;
        private readonly string _operationName;
        private readonly ResultCode _resultCode;

        public OperationResult(string operationName, ResultCode resultCode, string details = "")
        {
            _resultCode = resultCode;
            _details = details;
            _operationName = operationName;
        }

        public OperationResult(string operationName, ResultCode resultCode, string details, OperationResult[] children) : this(operationName, resultCode, details)
        {
            _children = children;
        }

        public string OperationName
        {
            get { return _operationName; }
        }

        public ResultCode ResultCode
        {
            get { return _resultCode; }
        }

        public string Details
        {
            get { return _details; }
        }
    }
}

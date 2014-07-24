using System.Linq;
using System.Text;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class OperationResult
    {
        private readonly OperationResult[] _children;
        private readonly string _details;
        private readonly string _operationName;
        private readonly ResultCode _resultCode;

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

        public OperationResult(string operationName, ResultCode resultCode, string details = "")
        {
            _operationName = operationName;
            _resultCode = resultCode;
            _details = details;
        }

        public OperationResult(string operationName, OperationResult[] children)
        {
            _operationName = operationName;
            _children = children;
            _resultCode = _children.Max(x => x.ResultCode);

            _details = BuildDetails();
        }

        private string BuildDetails()
        {
            var details = new StringBuilder();

            DescribeFail(details);

            DescribeResults(details);

            return details.ToString();
        }

        private void DescribeResults(StringBuilder details)
        {
            var template = _resultCode == ResultCode.Succeed 
                ? "Deployment task {0} succeed." 
                : "Deployment task {0} failed.";

            details
                .AppendFormat(template, _operationName)
                .AppendLine();
        }

        private void DescribeFail(StringBuilder details)
        {
            var failed = _children.FirstOrDefault(x => x.ResultCode == ResultCode.Failed);
            if (failed == null)
            {
                return;
            }
            details
                .AppendFormat("Operation {0} failed. Details:", failed._operationName)
                .AppendLine()
                .AppendLine(failed.Details);
        }
    }
}

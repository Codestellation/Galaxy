using System.Linq;
using System.Text;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager.Helpers
{
    public class ResultsDescriberHelper
    {
        public static OperationResult AggregateResults(DeploymentTask deploymentTask, OperationResult[] results)
        {
            StringBuilder details = new StringBuilder();

            var success = results.All(item => item.ResultType == OperationResultType.OR_OK);

            foreach (var result in results)
            {
                if (result.ResultType == OperationResultType.OR_FAIL)
                {
                    details.AppendLine(DescribeFail(result));
                }
            }

            details.AppendLine(DescribeResults(success, details, deploymentTask));

            var deploymentResult = 
                new OperationResult(
                    deploymentTask.Name, 
                    success ? OperationResultType.OR_OK : OperationResultType.OR_FAIL, 
                    details.ToString(), 
                    results);

            return deploymentResult;
        }

        private static string DescribeResults(bool success, StringBuilder details, DeploymentTask deploymentTask)
        {
            if (success)
                return string.Format("Deployment task {0} succeeded.", deploymentTask.Name);
            else
                return string.Format("Deployment task {0} failed.", deploymentTask.Name);
        }

        private static string DescribeFail(OperationResult operationResult)
        {
            return string.Format("Operation {0} failed. Details:\r\n{1}", 
                operationResult.OperationName,
                operationResult.Details);
        }

    }
}

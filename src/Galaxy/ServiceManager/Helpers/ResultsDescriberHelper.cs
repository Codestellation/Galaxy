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

            var success = results.All(item => item.ResultCode == ResultCode.Succeed);

            foreach (var result in results)
            {
                if (result.ResultCode == ResultCode.Failed)
                {
                    DescribeFail(details, result);
                }
            }

            DescribeResults(success, details, deploymentTask);

            var deploymentResult =
                new OperationResult(
                    deploymentTask.Name,
                    success ? ResultCode.Succeed : ResultCode.Failed,
                    details.ToString(),
                    results);

            return deploymentResult;
        }

        private static void DescribeResults(bool success, StringBuilder details, DeploymentTask deploymentTask)
        {
            var template = success ? "Deployment task {0} succeed." : "Deployment task {0} failed.";
            
            details
                .AppendFormat(template, deploymentTask)
                .AppendLine();
        }

        private static void DescribeFail(StringBuilder details, OperationResult operationResult)
        {
            details
                .AppendFormat("Operation {0} failed. Details:", operationResult.OperationName)
                .AppendLine()
                .AppendLine(operationResult.Details);
        }
    }
}

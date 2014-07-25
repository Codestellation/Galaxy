using System.Text;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class FakeOpSuccess : OperationBase
    {
        public FakeOpSuccess(string basePath, Deployment deployment) :
            base(basePath, deployment)
        {
        }

        public override void Execute(StringBuilder buildLog)
        {
        }
    }
}
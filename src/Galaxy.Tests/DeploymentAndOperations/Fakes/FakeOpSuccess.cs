using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class FakeOpSuccess : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
        }
    }
}
using System;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class FakeOpFail : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            throw new InvalidOperationException();
        }
    }
}
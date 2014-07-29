using System;
using System.IO;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class FakeOpFail : IOperation
    {
        public void Execute(TextWriter buildLog)
        {
            throw new InvalidOperationException();
        }
    }
}
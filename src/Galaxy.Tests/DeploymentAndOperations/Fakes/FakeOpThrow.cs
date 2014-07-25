using System;
using System.Text;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class FakeOpThrow : OperationBase
    {
        public FakeOpThrow(string basePath, Deployment deployment, NugetFeed feed) :
            base(basePath, deployment, feed)
        {
        }

        public override void Execute(StringBuilder buildLog)
        {
            throw new InvalidOperationException("thrown IOP");
        }
    }
}
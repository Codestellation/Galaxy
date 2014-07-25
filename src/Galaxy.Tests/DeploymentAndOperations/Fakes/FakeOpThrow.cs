using System;
using System.IO;
using System.Text;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class FakeOpThrow : OperationBase
    {
        public FakeOpThrow(string basePath, Deployment deployment) :
            base(basePath, deployment)
        {
        }

        public override void Execute(TextWriter buildLog)
        {
            throw new InvalidOperationException("thrown IOP");
        }
    }
}
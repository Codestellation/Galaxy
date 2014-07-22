using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class FakeOpThrow: OperationBase
    {
        public FakeOpThrow(string basePath, Deployment deployment, NugetFeed feed) :
            base(basePath, deployment, feed)
        {
        }

        public override void Execute()
        {
            throw new InvalidOperationException("thrown IOP");
        }
    }
}

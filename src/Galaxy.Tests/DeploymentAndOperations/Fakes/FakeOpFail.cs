using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class FakeOpFail: OperationBase
    {
        public FakeOpFail(string basePath, Deployment deployment, NugetFeed feed) :
            base(basePath, deployment, feed)
        {
        }

        public override void Execute()
        {
            throw new InvalidOperationException();
        }
    }
}

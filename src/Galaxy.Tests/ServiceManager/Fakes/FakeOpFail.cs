using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestellation.Galaxy.Tests.ServiceManager.Fakes
{
    public class FakeOpFail: ServiceOperation
    {
        public FakeOpFail(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {
        }

        public override void Execute()
        {
            StoreResult(OperationResult.OR_FAIL, "Op details");
        }
    }
}

using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestellation.Galaxy.Tests.ServiceManager.Fakes
{
    public class FakeOpThrow: ServiceOperation
    {
        public FakeOpThrow(string targetPath, ServiceApp serviceApp, NugetFeed feed) :
            base(targetPath, serviceApp, feed)
        {
        }

        public override void Execute()
        {
            throw new InvalidOperationException("thrown IOP");
        }
    }
}

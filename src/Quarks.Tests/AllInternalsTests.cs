using System.Linq;
using Codestellation.Quarks.IO;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests
{
    [TestFixture]
    public class AllInternalsTests
    {
        [Test]
        public void All_types_from_quarks_must_be_internal()
        {
            var nonInternalTypes =
                typeof (Folder)
                    .Assembly
                    .GetTypes()
                    .Where(x => x.IsPublic)
                    .ToList();


            Assert.That(nonInternalTypes, Is.Empty, "All types should be internals");
        }
    }
}
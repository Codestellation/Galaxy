using Codestellation.Quarks.Enumerations;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.Enumerations
{
    [TestFixture]
    public class EnumUtil
    {
        public enum YesNo
        {
            Yes,
            No
        }
        

        [Test]
        public void Should_return_same_instance_of_string()
        {
            var first = YesNo.Yes.AsString();
            var second = YesNo.Yes.AsString();

            Assert.That(first, Is.SameAs(second));
        }
    }
}
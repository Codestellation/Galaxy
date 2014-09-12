using System;
using System.Linq;
using Codestellation.Quarks.Collections;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void Should_return_empty_if_original_is_null()
        {
            object[] self = null;
            Assert.That(self.EmptyIfNull(), Is.EqualTo(Enumerable.Empty<object>()));
        }

        [Test]
        public void Should_return_original_if_original_is_not_null()
        {
            object[] self = {123, "Hello", DateTime.Now};
            Assert.That(self.EmptyIfNull(), Is.EqualTo(self));
        }
    }
}
using Codestellation.Quarks.StringUtils;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.StringUtils
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void Should_format_string_with()
        {
            const string template = "Hello, {0}!";
            Assert.That(template.FormatWith("World"), Is.EqualTo("Hello, World!"));
        }
    }
}
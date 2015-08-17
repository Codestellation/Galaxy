using Codestellation.Galaxy.Host.ConfigManagement;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.Host.ConfigManagement
{
    [TestFixture]
    public class ConfigElementTests
    {
        [Test]
        public void Should_parse_property()
        {
            var element = new ConfigElement(Test.ConsulName, Test.PortProperty);

            Assert.That(element.Key, Is.EqualTo("port"));
            Assert.That(element.Path, Is.EqualTo("company/environment/product/service/port"));
            Assert.That(element.Property, Is.EqualTo(Test.PortProperty));
        }
    }
}
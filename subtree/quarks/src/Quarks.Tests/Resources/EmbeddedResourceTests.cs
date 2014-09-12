using System;
using System.Reflection;
using Codestellation.Quarks.Resources;
using NUnit.Framework;

namespace Codestellation.Quarks.Tests.Resources
{
    [TestFixture]
    public class EmbeddedResourceTests
    {
        [Test]
        public void Should_find_resource_and_return_stream_from_calling_assembly()
        {
            var name = "embeddedsample.txt";

            var resource = EmbeddedResource.EndsWith(name, StringComparison.OrdinalIgnoreCase);

            Assert.That(resource, Is.Not.Null);
        }

        [Test]
        public void Should_find_resource_and_return_stream_from_specified_assembly()
        {
            var name = "embeddedsample.txt";

            var resource = EmbeddedResource.EndsWith(name, Assembly.GetExecutingAssembly(), StringComparison.OrdinalIgnoreCase);

            Assert.That(resource, Is.Not.Null);
        }

        [Test]
        public void Should_throw_if_resource_not_found()
        {
            var name = "embeddedsample.txt";

            Assert.Throws<InvalidOperationException>(() => EmbeddedResource.EndsWith(name, StringComparison.Ordinal));
        }

        [Test]
        public void Should_enumerate_all_embedded_resources_from_calling_assembly()
        {
            var resources = EmbeddedResource.GetAll();

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Length, Is.EqualTo(1));
        }

        [Test]
        public void Should_enumerate_all_embedded_resources_from_specified_assembly()
        {
            var resources = EmbeddedResource.GetAll(Assembly.GetExecutingAssembly());

            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.Length, Is.EqualTo(1));
        }
    }
}
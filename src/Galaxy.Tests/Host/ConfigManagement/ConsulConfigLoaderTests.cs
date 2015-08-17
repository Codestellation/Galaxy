using System;
using Codestellation.Galaxy.Host.ConfigManagement;
using Consul;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.Host.ConfigManagement
{
    [TestFixture]
    public class ConsulConfigLoaderTests
    {
        private ConsulWrapper _consul;
        private Client _client;
        private ConfigLoader _loader;
        private ConfigElement _element;
        private const int TestPort = 10;

        [SetUp]
        public void Setup()
        {
            _consul = new ConsulWrapper();
            _consul.Start();

            _client = new Client();
            _loader = new ConfigLoader(_client);
            _element = new ConfigElement(Test.ConsulName, Test.PortProperty);
        }

        [TearDown]
        public void TearDown()
        {
            _consul.Stop();
        }

        [Test]
        public void Should_be_missed_after_load()
        {
            _loader.Load(_element);
            Assert.That(_element.IsMissed, Is.True);
            Assert.That(_element.IsLoaded, Is.True);

            Assert.Throws<InvalidOperationException>(() => { var x = _element.RawValue; });
            Assert.Throws<InvalidOperationException>(() => { var x = _element.Value; });
        }

        [Test]
        public void Should_be_invalid_if_value_is_not_convertible_to_property_value()
        {
            _consul.PutKey(_element.Path, new object());
            _loader.Load(_element);

            Assert.That(_element.IsLoaded, Is.True);
            Assert.That(_element.IsMissed, Is.False);
            Assert.That(_element.IsValid, Is.False);

            Assert.That(_element.RawValue, Is.Not.Null);
            Assert.That(_element.Message, Is.Not.Empty);
        }

        [Test]
        public void Should_load_simple_value_from_consul()
        {
            _consul.PutKey(_element.Path, TestPort);
            _loader.Load(_element);

            Assert.That(_element.IsLoaded, Is.True);
            Assert.That(_element.IsMissed, Is.False);
            Assert.That(_element.IsValid, Is.True);
            Assert.That(_element.RawValue, Is.Not.Null);
            Assert.That(_element.Value, Is.EqualTo(TestPort));
        }
    }
}
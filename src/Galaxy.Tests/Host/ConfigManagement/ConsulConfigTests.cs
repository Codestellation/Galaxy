using System;
using System.Linq;
using Codestellation.Galaxy.Host.ConfigManagement;
using Consul;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.Host.ConfigManagement
{
    [TestFixture]
    public class ConsulConfigTests
    {
        private ConsulWrapper _consul;
        private Client _client;
        private ConfigLoader _loader;
        private ConsulConfig _config;

        [SetUp]
        public void Setup()
        {
            _consul = new ConsulWrapper();
            _consul.Start();

            _client = new Client();
            _loader = new ConfigLoader(_client);
            _config = new ConsulConfig(typeof(SampleConfig), _loader, Test.ConsulName);
        }

        [TearDown]
        public void TearDown()
        {
            _consul.Stop();
        }

        [Test]
        public void Should_parse_config_to_generate_kes()
        {
            Assert.That(_config.Elements.Select(x => x.Key), Contains.Item(Test.PortKey));
            Assert.That(_config.Elements.Select(x => x.Key), Contains.Item(Test.HostKey));
        }

        [Test]
        public void Should_be_invalid_if_any_element_invalid()
        {
            //given
            _consul.PutKey(_config.Elements.First(x => x.Key == Test.PortKey).Path, 10);

            //when
            _config.Load();

            Console.WriteLine(_config.ValidationResult.ToString());

            //then
            var result = _config.ValidationResult;

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void Should_be_valid_if_all_elements_are_valid()
        {
            //given
            _consul.PutKey(_config.Elements.First(x => x.Key == Test.PortKey).Path, 10);
            _consul.PutKey(_config.Elements.First(x => x.Key == Test.HostKey).Path, "localhost");

            //when 
            _config.Load();

            //then
            var result = _config.ValidationResult;
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void Should_buildi_config_of_specified_type()
        {
            //given
            _consul.PutKey(_config.Elements.First(x => x.Key == Test.PortKey).Path, 10);
            _consul.PutKey(_config.Elements.First(x => x.Key == Test.HostKey).Path, "localhost");

            //when 
            _config.Load();

            //then
            var sample = (SampleConfig)_config.BuildConfig();

            Assert.That(sample.Port, Is.EqualTo(10));
            Assert.That(sample.Host, Is.EqualTo("localhost"));
        }
    }
}
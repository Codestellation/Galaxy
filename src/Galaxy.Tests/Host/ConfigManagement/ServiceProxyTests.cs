using System;
using System.IO;
using System.Linq;
using Codestellation.Galaxy.Host;
using Codestellation.Galaxy.Host.ConfigManagement;
using Consul;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.Host.ConfigManagement
{
    [TestFixture]
    public class ServiceProxyTests
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

            _consul.PutKey(_config.Elements.First(x => x.Key == Test.PortKey).Path, 10);
            _consul.PutKey(_config.Elements.First(x => x.Key == Test.HostKey).Path, "localhost");

            var config = new
            {
                Logs = AppDomain.CurrentDomain.BaseDirectory,
                Configs = AppDomain.CurrentDomain.BaseDirectory,
                Data = AppDomain.CurrentDomain.BaseDirectory,
                Consul = Test.ConsulSettings
            };
            File.WriteAllText("config.json", JsonConvert.SerializeObject(config));
        }

        [TearDown]
        public void TearDown()
        {
            _consul.Stop();
        }

        [Test]
        public void Should_give_loaded_config_to_service()
        {
            var proxyType = typeof(IService).Assembly.GetTypes().Single(x => x.Name == "ServiceProxy");

            var proxy = proxyType.GetConstructor(new[] { typeof(Type) }).Invoke(new[] { typeof(SampleService) });

            var sampleService = (SampleService)proxyType.GetProperty("Service").GetValue(proxy);

            var config = sampleService.Config;

            Assert.That(config.Port, Is.EqualTo(10));
            Assert.That(config.Host, Is.EqualTo("localhost"));
        }
    }
}
using System;
using System.IO;
using System.Linq;
using Codestellation.Galaxy.Host.ConfigManagement;
using Consul;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.Host
{
    internal class ServiceProxy
    {
        private readonly IService _service;

        public ServiceProxy(Type serviceType)
        {
            _service = (IService)Activator.CreateInstance(serviceType);

            TryLoadConsulConfig();
        }

        public IService Service
        {
            get { return _service; }
        }

        private void TryLoadConsulConfig()
        {
            var consulAware = Service
                .GetType()
                .GetInterfaces()
                .SingleOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConsulConfigAware<>));

            if (consulAware != null)
            {
                var consulConfigContent = File.ReadAllText("consul.json");

                var consulSettings = JsonConvert.DeserializeObject<ConsulConfigSettings>(consulConfigContent);

                var configType = consulAware.GenericTypeArguments[0];

                var clientConfig = new ConsulClientConfiguration();
                if (!string.IsNullOrWhiteSpace(consulSettings.Address))
                {
                    clientConfig.Address = consulSettings.Address;
                }

                var client = new Client(clientConfig);
                var loader = new ConfigLoader(client);
                var config = new ConsulConfig(configType, loader, consulSettings.Name);

                config.Load();

                if (!config.ValidationResult.IsValid)
                {
                    throw new InvalidOperationException(config.ValidationResult.ToString());
                }

                var acceptMethod = consulAware.GetMethod("Accept");

                var result = (ValidationResult)acceptMethod.Invoke(Service, new[] { config.BuildConfig() });

                if (!result.IsValid)
                {
                    throw new InvalidOperationException("Invalid consul config");
                }
            }
        }

        public void Start()
        {
            Service.Start();
        }

        public void Stop()
        {
            Service.Stop();
        }
    }
}
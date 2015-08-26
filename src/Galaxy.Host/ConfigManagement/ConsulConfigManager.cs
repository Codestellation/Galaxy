using System;
using System.IO;
using System.Linq;
using Codestellation.Quarks.IO;
using Consul;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.Host.ConfigManagement
{
    public class ConsulConfigManager
    {
        private static readonly string ConsulSettingsFileName = Folder.ToFullPath("consul.json");
        private static readonly string DevConfigFileName = Folder.ToFullPath("config.dev.json");

        public static void TryLoadConsulConfig(IService service)
        {
            var consulAware = service
                .GetType()
                .GetInterfaces()
                .SingleOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConsulConfigAware<>));

            if (consulAware == null)
            {
                return;
            }

            if (TryLoadConsulConfig(service, consulAware))
            {
                return;
            }

            if (TryLoadDevConfig(service, consulAware))
            {
                return;
            }

            throw new InvalidOperationException("Could not find service config");
        }

        private static bool TryLoadConsulConfig(IService service, Type consulAware)
        {
            if (!File.Exists(ConsulSettingsFileName))
            {
                return false;
            }
            var consulConfigContent = File.ReadAllText(ConsulSettingsFileName);

            var consulSettings = JsonConvert.DeserializeObject<ConsulConfigSettings>(consulConfigContent);

            Type configType = GetConfigType(consulAware);

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

            var serviceConfig = config.BuildConfig();
            ProvideConfigToService(service, consulAware, serviceConfig);
            return true;
        }

        private static bool TryLoadDevConfig(IService service, Type consulAware)
        {
            if (!Environment.UserInteractive)
            {
                return false;
            }

            if (!File.Exists(DevConfigFileName))
            {
                return false;
            }

            var content = File.ReadAllText(DevConfigFileName);
            var configType = GetConfigType(consulAware);

            var serviceConfig = JsonConvert.DeserializeObject(content, configType);

            ProvideConfigToService(service, consulAware, serviceConfig);

            return true;
        }

        private static void ProvideConfigToService(IService service, Type consulAware, object serviceConfig)
        {
            var acceptMethod = consulAware.GetMethod("Accept");

            var result = (ValidationResult)acceptMethod.Invoke(service, new[] { serviceConfig });

            if (!result.IsValid)
            {
                throw new InvalidOperationException("Invalid consul config");
            }
        }

        private static Type GetConfigType(Type consulAware)
        {
            var configType = consulAware.GenericTypeArguments[0];
            return configType;
        }
    }
}
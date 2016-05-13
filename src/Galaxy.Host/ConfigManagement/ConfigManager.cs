using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.Host.ConfigManagement
{
    public static class ConfigManager
    {
        public static void TryLoadConfig(IService service)
        {
            Type configAware = ConfigAware(service);

            if (configAware == null)
            {
                return;
            }

            HostConfig hostConfig = service.HostConfig;

            LoadFileConfig(service, configAware, hostConfig);
        }

        private static Type ConfigAware(IService service)
        {
            var configAware = service
                .GetType()
                .GetInterfaces()
                .SingleOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConfigAware<>));
            return configAware;
        }

        private static void LoadFileConfig(IService service, Type consulAware, HostConfig hostConfig)
        {
            var serviceConfigFile = new FileInfo(Path.Combine(hostConfig.Configs.FullName, "config.json"));

            var configType = GetConfigType(consulAware);

            if (!serviceConfigFile.Exists)
            {
                throw new FileNotFoundException($"Could not find config file '${serviceConfigFile.FullName}'", serviceConfigFile.FullName);
            }

            var serviceConfig = ReadConfig(configType, serviceConfigFile);
            ProvideConfigToService(service, consulAware, serviceConfig);
        }

        private static object ReadConfig(Type configType, FileInfo serviceConfigFile)
        {
            var content = File.ReadAllText(serviceConfigFile.FullName);
            var serviceConfig = JsonConvert.DeserializeObject(content, configType);
            return serviceConfig;
        }

        private static void ProvideConfigToService(IService service, Type consulAware, object serviceConfig)
        {
            var acceptMethod = consulAware.GetMethod("Accept");

            var result = (ValidationResult)acceptMethod.Invoke(service, new[] { serviceConfig });

            if (!result.IsValid)
            {
                throw new InvalidOperationException("Invalid consul config: " + result);
            }
        }

        private static Type GetConfigType(Type consulAware)
        {
            var configType = consulAware.GenericTypeArguments[0];
            return configType;
        }
    }
}
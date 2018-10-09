using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        private static void LoadFileConfig(IService service, Type configAware, HostConfig hostConfig)
        {
            var serviceConfigFile = new FileInfo(Path.Combine(hostConfig.Configs.FullName, "config.json"));

            Type configType = GetConfigType(configAware);

            if (!serviceConfigFile.Exists)
            {
                throw new FileNotFoundException($"Could not find config file '${serviceConfigFile.FullName}'", serviceConfigFile.FullName);
            }

            object serviceConfig = ReadConfig(configType, serviceConfigFile);
            ProvideConfigToService(service, configAware, serviceConfig);
        }

        private static object ReadConfig(Type configType, FileInfo serviceConfigFile)
        {
            var content = File.ReadAllText(serviceConfigFile.FullName);
            var serviceConfig = JsonConvert.DeserializeObject(content, configType);
            return serviceConfig;
        }

        private static void ProvideConfigToService(IService service, Type configAware, object serviceConfig)
        {
            MethodInfo acceptMethod = configAware.GetMethod("Accept");
            if (acceptMethod == null)
            {
                throw new InvalidOperationException($"Accept method not found for {service.GetType()}");
            }

            var result = (ValidationResult)acceptMethod.Invoke(service, new[] {serviceConfig});

            if (!result.IsValid)
            {
                throw new InvalidOperationException("Invalid config: " + result);
            }
        }

        private static Type GetConfigType(Type configAware)
        {
            Type configType = configAware.GenericTypeArguments[0];
            return configType;
        }

        public static string GetSample(IService service)
        {
            Type configAware = ConfigAware(service);
            dynamic dynService = service;
            if (configAware != null && dynService.CanGetSample)
            {
                object sample = dynService.GetSample();

                JObject jSample = JObject.FromObject(sample);

                return jSample.ToString(Formatting.Indented);
            }
            return "{}";
        }
    }
}
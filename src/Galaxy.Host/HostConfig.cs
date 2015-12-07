using System;
using System.IO;
using System.Reflection;
using Codestellation.Galaxy.Host.Misc;
using Codestellation.Quarks.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Codestellation.Galaxy.Host
{
    public class HostConfig
    {
        private static readonly FileInfo ConfigFile = new FileInfo(Folder.ToFullPath("host.config.json"));

        public static readonly HostConfig DevConfig = new HostConfig
        {
            Data = new DirectoryInfo(Folder.ToFullPath("data")),
            Logs = new DirectoryInfo(Folder.ToFullPath("logs")),
            Configs = new DirectoryInfo(Folder.ToFullPath("configs"))
        };

        public ConsulConfigSettings Consul { get; set; }

        public DirectoryInfo Logs { get; private set; }

        public DirectoryInfo Configs { get; private set; }

        public DirectoryInfo Data { get; private set; }

        public bool UseConsulConfig => !string.IsNullOrWhiteSpace(Consul?.Name);

        public static HostConfig Load()
        {
            if (ConfigFile.Exists)
            {
                var content = File.ReadAllText(ConfigFile.FullName);

                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new DirectoryInfoConverter());
                var resolver = new DefaultContractResolver();
                resolver.DefaultMembersSearchFlags = resolver.DefaultMembersSearchFlags | BindingFlags.NonPublic;
                settings.ContractResolver = resolver;

                return JsonConvert.DeserializeObject<HostConfig>(content, settings);
            }

            if (Environment.UserInteractive)
            {
                return DevConfig;
            }

            throw new InvalidOperationException("host.config.json not found");
        }

        internal void Validate()
        {
            EnsureExists(nameof(Configs), Configs);
            EnsureExists(nameof(Data), Data);
            EnsureExists(nameof(Logs), Logs);
        }

        private void EnsureExists(string propertyName, DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new InvalidOperationException($"Property ${propertyName} is not initialized");
            }
            Folder.EnsureExists(directoryInfo.FullName);
        }
    }
}
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

        private readonly object _latch = new object();

        public ConsulConfigSettings Consul { get; set; }

        public DirectoryInfo Logs { get; private set; }

        public DirectoryInfo Configs { get; private set; }

        public DirectoryInfo Data { get; private set; }

        public bool UseConsulConfig => !string.IsNullOrWhiteSpace(Consul?.Name);

        public void SaveData(ISettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            var fullpath = MakeFilename(settings);

            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            lock (_latch)
            {
                File.WriteAllText(fullpath, json);
            }
        }

        public TSettings LoadData<TSettings>()
            where TSettings : ISettings, new()
        {
            var settings = new TSettings();
            var fullpath = MakeFilename(settings);

            lock (_latch)
            {
                if (File.Exists(fullpath))
                {
                    var json = File.ReadAllText(fullpath);
                    return JsonConvert.DeserializeObject<TSettings>(json);
                }
            }
            return settings;
        }

        private string MakeFilename(ISettings settings)
        {
            var filename = settings.Filename;

            if (Path.IsPathRooted(filename))
            {
                throw new InvalidOperationException("ISettings.Filename must be not rooted path");
            }

            var fullpath = Folder.Combine(Data.FullName, filename);
            return fullpath;
        }

        internal static HostConfig Load()
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
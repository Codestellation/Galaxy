using System.IO;
using System.Reflection;
using Topshelf.HostConfigurators;

namespace Codestellation.Galaxy.Host.LogManagement
{
    internal static class LoggingExtensions
    {
        public static void InitializeLoggers(this HostConfigurator configurator, Assembly assembly, DirectoryInfo logs)
        {
            NLogManager.TryInitialize(configurator, assembly, logs);
        }
    }
}
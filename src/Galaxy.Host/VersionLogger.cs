using System.Reflection;
using Codestellation.Galaxy.Host.Misc;
using Topshelf.Logging;

namespace Codestellation.Galaxy.Host
{
    public static class VersionLogger
    {
        public static void LogVersions()
        {
            var logWriter = HostLogger.Get(typeof(Run));

            var hostVersion = GetVersion(Assembly.GetExecutingAssembly());
            var hostVersionMessage = $"Host version: {hostVersion}";
            logWriter.Info(hostVersionMessage);

            var serviceVersion = GetVersion(Assembly.GetEntryAssembly());
            var serviceVersionMessage = $"Service version: {serviceVersion}";
            logWriter.Info(serviceVersionMessage);
        }

        private static string GetVersion(Assembly assembly)
        {
            var info = assembly.GetAttribute<AssemblyInformationalVersionAttribute>();

            if (info != null)
            {
                return info.InformationalVersion;
            }

            var version = assembly.GetAttribute<AssemblyVersionAttribute>();

            return version != null ? version.Version : "Unknown";
        }
    }
}
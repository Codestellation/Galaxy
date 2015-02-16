using System.Reflection;

namespace Codestellation.Quarks.Reflection
{
    internal static class AssemblyVersion
    {
        public static readonly string InformationalVersion;

        public static readonly string Version;
        
        static AssemblyVersion()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();

            var asmInfoVersion = entryAssembly.GetAttribute<AssemblyInformationalVersionAttribute>();
            var asmVersion = entryAssembly.GetAttribute<AssemblyVersionAttribute>();

            InformationalVersion = asmInfoVersion == null ? "unknown" : asmInfoVersion.InformationalVersion;
            Version = asmVersion == null ? "unknown" : asmVersion.Version;
        }
    }
}
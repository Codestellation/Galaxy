using Codestellation.Quarks.Reflection;

namespace Codestellation.Galaxy.Infrastructure
{
    public static class ServiceVersion
    {
        public static string InformationalVersion
        {
            get { return AssemblyVersion.InformationalVersion; }
        }
    }
}
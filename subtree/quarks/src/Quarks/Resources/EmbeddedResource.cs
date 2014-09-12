using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Codestellation.Quarks.Resources
{
    internal static class EmbeddedResource
    {
        public static Stream EndsWith(string name, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            //TODO: throw more informative exceptions
            Assembly assembly = Assembly.GetCallingAssembly();

            return EndsWith(name, assembly, comparison);
        }

        public static Stream EndsWith(string name, Assembly assembly, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            string resourceName = assembly
                .GetManifestResourceNames()
                .Single(x => x.EndsWith(name, comparison));

            return assembly.GetManifestResourceStream(resourceName);
        }

        public static string[] GetAll()
        {
            var assembly = Assembly.GetCallingAssembly();
            return GetAll(assembly);
        }

        public static string[] GetAll(Assembly assembly)
        {
            string[] names = assembly.GetManifestResourceNames();
            return names;
        }
    }
}
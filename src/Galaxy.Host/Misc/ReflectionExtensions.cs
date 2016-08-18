using System;
using System.Linq;
using System.Reflection;

namespace Codestellation.Galaxy.Host.Misc
{
    public static class ReflectionExtensions
    {
        public static T GetAttribute<T>(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return assembly
                .GetCustomAttributes(typeof(T), false)
                .Cast<T>()
                .FirstOrDefault();
        }

        public static Type ConfigAware(this Type type)
        {
            var configAware = type
                .GetInterfaces()
                .SingleOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConfigAware<>));
            return configAware;
        }
    }
}
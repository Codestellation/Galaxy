using System;
using System.Linq;
using System.Reflection;

namespace Codestellation.Quarks.Reflection
{
    internal static class TypeExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider self, bool inherited = false)
            where TAttribute : Attribute
        {
            return self.GetCustomAttributes(typeof (TAttribute), inherited).Cast<TAttribute>().SingleOrDefault();
        }
    }
}
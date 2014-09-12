using System;
using System.Collections.Generic;

namespace Codestellation.Quarks.Collections
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> self)
        {
            return self ?? CollectionExtensions.ArrayOf<T>.Empty;
        }

        public static string ToJoinedString<T>(this IEnumerable<T> self)
        {
            return String.Join(",", self);
        }
    }
}
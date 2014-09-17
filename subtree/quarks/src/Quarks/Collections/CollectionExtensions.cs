using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Codestellation.Quarks.Collections
{
    internal static class CollectionExtensions
    {
        private static readonly ConcurrentDictionary<Expression, Tuple<Delegate, Delegate>> ComparisonCache = new ConcurrentDictionary<Expression, Tuple<Delegate, Delegate>>();

        internal static class ArrayOf<T>
        {
            public static readonly T[] EmptyArray = new T[0];
        }
        
        public static TOutput[] ConvertToArray<TInput, TOutput>(this TInput[] self, Func<TInput, TOutput> converter)
        {
            return ConvertToArray(self, converter, self.Length);
        }

        public static TOutput[] ConvertToArray<TInput, TOutput>(this ICollection<TInput> self, Func<TInput, TOutput> converter)
        {
            return ConvertToArray(self, converter, self.Count);
        }

        public static TOutput[] ConvertToArray<TInput, TOutput>(this IEnumerable<TInput> self, Func<TInput, TOutput> converter, int arraySize)
        {
            var result = new TOutput[arraySize];

            int index = 0;
            foreach (var input in self)
            {
                result[index++] = converter(input);
            }

            return result;
        }

        public static List<TOutput> ConvertToList<TInput, TOutput>(this ICollection<TInput> self, Func<TInput, TOutput> converter)
        {
            var result = new List<TOutput>(self.Count);

            foreach (var input in self)
            {
                result.Add(converter(input));
            }

            return result;
        }

        public static bool NotEmpty<TItem>(this TItem[] self)
        {
            return self.Length > 0;
        }

        public static bool NotEmpty(this ICollection self)
        {
            return self.Count > 0;
        }

        public static bool Empty<TItem>(this TItem[] self)
        {
            return self.Length == 0;
        }

        public static bool Empty<TItem>(this ICollection<TItem> self)
        {
            return self.Count == 0;
        }

        public static T[] EmptyIfNull<T>(this T[] self)
        {
            return self ?? ArrayOf<T>.EmptyArray;
        }

        public static TItem ArrayFirst<TItem>(this TItem[] self)
        {
            return self[0];
        }

        public static TItem ListFirst<TItem>(this IList<TItem> self)
        {
            return self[0];
        }

        public static T[] CollectNotNull<T>(params T[] items) where T : class
        {
            if (items.Length == 0)
            {
                return items;
            }
            var count = 0;

            for (int index = 0; index < items.Length; index++)
            {
                if (items[index] != null)
                {
                    count++;
                }
            }

            var result = new T[count];

            count = 0;

            for (int index = 0; index < items.Length; index++)
            {
                var item = items[index];
                
                if (item != null)
                {
                    result[count++] = item;
                }
            }
            return result;
        }

        public static T[] CollectNotNull<T>(T item1) where T : class
        {
            var count = 0;
            if (item1 != null)
            {
                count++;
            }

            var result = new T[count];

            count = 0;
            if (item1 != null)
            {
                result[count++] = item1;
            }

            return result;
        }

        public static T[] CollectNotNull<T>(T item1, T item2) where T : class
        {
            var count = 0;
            if (item1 != null)
            {
                count++;
            }

            if (item2 != null)
            {
                count++;
            }

            var result = new T[count];

            count = 0;
            if (item1 != null)
            {
                result[count++] = item1;
            }

            if (item2 != null)
            {
                result[count++] = item2;
            }

            return result;
        }

        public static T[] CollectNotNull<T>(T item1, T item2, T item3) where T : class
        {
            var count = 0;
            if (item1 != null)
            {
                count++;
            }

            if (item2 != null)
            {
                count++;
            }

            if (item3 != null)
            {
                count++;
            }

            var result = new T[count];

             count = 0;
            if (item1 != null)
            {
                result[count++] = item1;
            }

            if (item2 != null)
            {
                result[count++] = item2;
            }

            if (item3 != null)
            {
                result[count++] = item3;
            }
            return result;
        }

        public static TInput[] SortAscending<TInput, TProperty>(this TInput[] input, Expression<Func<TInput, TProperty>> property)
        {
            var comparisons = GetOrCreateComparison(property);
            var ascending = comparisons.Item1;
            Array.Sort(input, (Comparison<TInput>)ascending);
            return input;
        }

        public static TInput[] SortDescending<TInput, TProperty>(this TInput[] input, Expression<Func<TInput, TProperty>> property)
        {
            var comparisons = GetOrCreateComparison(property);
            var descending = comparisons.Item2;
            Array.Sort(input, (Comparison<TInput>)descending);
            return input;
        }

        private static Tuple<Delegate, Delegate> GetOrCreateComparison<TInput, TProperty>(Expression<Func<TInput, TProperty>> property)
        {
            Tuple<Delegate, Delegate> result;
            if (!ComparisonCache.TryGetValue(property, out result))
            {
                result = BuildComparison(property);
                ComparisonCache[property] = result;
            }
            return result;
        }

        private static Tuple<Delegate,Delegate> BuildComparison<TInput, TProperty>(Expression<Func<TInput, TProperty>> property)
        {
            var getter = property.Compile();
            Comparison<TInput> ascending = (x, y) => Comparer<TProperty>.Default.Compare(getter(x), getter(y));
            Comparison<TInput> descending = (x, y) => Comparer<TProperty>.Default.Compare(getter(y), getter(x));
            return new Tuple<Delegate, Delegate>(ascending,descending);
        }
    }
}
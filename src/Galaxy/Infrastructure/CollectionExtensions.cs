using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Codestellation.Galaxy.Infrastructure
{
    public static class CollectionExtensions
    {
        private static ConcurrentDictionary<Expression, Delegate> _comparisonCache;

        static CollectionExtensions()
        {
            _comparisonCache = new ConcurrentDictionary<Expression, Delegate>();
        }
        
        public static TOutput[] ConvertToArray<TInput, TOutput>(this IReadOnlyCollection<TInput> self, Func<TInput, TOutput> converter)
        {
            var outputs = new TOutput[self.Count];

            int index = 0;
            foreach (TInput input in self)
            {
                TOutput model = converter(input);
                outputs[index] = model;
                index++;
            }
            return outputs;
        }

        public static TInput[] SortAscending<TInput, TProperty>(this TInput[] input, Expression<Func<TInput, TProperty>> property)
        {
            var comparison = GetOrCreateComparison(property);
            Array.Sort(input, comparison);
            return input;
        }

        public static TInput[] SortDescending<TInput, TProperty>(this TInput[] input, Expression<Func<TInput, TProperty>> property)
        {
            var ascending = GetOrCreateComparison(property);
            Comparison<TInput> comparison = (x, y) => -ascending(x,y);
            Array.Sort(input, comparison);
            return input;
        }
        private static Comparison<TInput> GetOrCreateComparison<TInput, TProperty>(Expression<Func<TInput, TProperty>> property)
        {
            Delegate result;
            if (!_comparisonCache.TryGetValue(property, out result))
            {
                result = BuildComparison(property);
                _comparisonCache[property] = result;
            }
            return (Comparison<TInput>) result;
        }

        private static Delegate BuildComparison<TInput, TProperty>(Expression<Func<TInput, TProperty>> property)
        {
            var getter = property.Compile();
            Comparison<TInput> result = (x, y) => Comparer<TProperty>.Default.Compare(getter(x), getter(y));
            return result;
        }
    }
}
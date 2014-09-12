using System;
using System.Collections.Generic;
using System.Threading;

namespace Codestellation.Quarks.Collections
{
    internal static class DictionaryExtensions
    {
        public static bool ThreadSafeAdd<TKey, TValue>(ref Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            Dictionary<TKey, TValue> afterCas;
            Dictionary<TKey, TValue> beforeCas;
            do
            {
                beforeCas = dictionary;
                Thread.MemoryBarrier();
                if (dictionary.ContainsKey(key)) return false;

                var newDictionary = new Dictionary<TKey, TValue>(dictionary, dictionary.Comparer) { { key, value } };

                afterCas = Interlocked.CompareExchange(ref dictionary, newDictionary, beforeCas);
            } while (beforeCas != afterCas);
            return true;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, Func<TKey, TValue> factory)
        {
            TValue value;
            if (self.TryGetValue(key, out value) == false)
            {
                value = factory(key);
                self.Add(key, value);
            }
            return value;
        } 
    }
}
using System.Collections.Generic;
using System.Threading;

namespace Codestellation.Quarks.Collections
{
    internal static class HashSetExtensions
    {
        public static bool ThreadSafeAdd<TItem>(ref HashSet<TItem> set, TItem item)
        {
            HashSet<TItem> afterCas;
            HashSet<TItem> beforeCas;
            do
            {
                beforeCas = set;
                Thread.MemoryBarrier();
                if (set.Contains(item)) return false;

                var newSet = new HashSet<TItem>(set, set.Comparer) { item };

                afterCas = Interlocked.CompareExchange(ref set, newSet, beforeCas);
            } while (beforeCas != afterCas);
            return true;
        }

        public static bool ThreadSafeRemove<TItem>(ref HashSet<TItem> set, TItem item)
        {
            HashSet<TItem> afterCas;
            HashSet<TItem> beforeCas;
            do
            {
                beforeCas = set;
                Thread.MemoryBarrier();

                if (!set.Contains(item)) return false;

                var newSet = new HashSet<TItem>(set, set.Comparer);
                newSet.Remove(item);

                afterCas = Interlocked.CompareExchange(ref set, newSet, beforeCas);
            } while (beforeCas != afterCas);
            return true;
        }
    }
}
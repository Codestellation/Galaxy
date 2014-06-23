using Nejdb;
using Nejdb.Queries;

namespace Codestellation.Galaxy.Infrastructure
{
    public static class EjdbCollectionExtensions
    {
        public static TItem[] PerformQuery<TItem>(this Collection self, QueryBuilder queryBuilder = null)
        {
            using (var query = queryBuilder == null ? self.CreateQuery<TItem>() : self.CreateQuery<TItem>(queryBuilder))

            using (var cursor = query.Execute())
            {
                var resultsAsArray = new TItem[cursor.Count];
                for (int resultIndex = 0; resultIndex < cursor.Count; resultIndex++)
                {
                    resultsAsArray[resultIndex] = cursor[resultIndex];
                }
                return resultsAsArray;
            }

        }
    }
}
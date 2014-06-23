using System;
using System.Collections.Generic;

namespace Codestellation.Galaxy.Infrastructure
{
    public static class CollectionExtensions
    {
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
    }
}
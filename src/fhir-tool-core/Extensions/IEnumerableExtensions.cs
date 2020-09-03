using System;
using System.Collections.Generic;
using System.Linq;

namespace FhirTool.Core.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int batchSize)
        {
            return items.Select((item, index) => new { item, index })
                        .GroupBy(it => it.index / batchSize)
                        .Select(grp => grp.Select(it => it.item));
        }
    }
}

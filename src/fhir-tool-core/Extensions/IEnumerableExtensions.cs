/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

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

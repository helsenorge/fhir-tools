using System;
using System.Collections.Generic;
using System.Linq;

namespace FhirTool.Conversion
{
    public static class DictionaryExtensions
    {
        public static void Add<TTo, TFrom>(this Dictionary<(Type, Type), Delegate> me, Action<TTo, TFrom, FhirConverter> func)
        {
            me.Add((typeof(TTo), typeof(TFrom)), func);
        }

        public static Dictionary<T2,T1> Reverse<T1,T2>(this Dictionary<T1,T2> me)
        {
            return me.ToDictionary(e => e.Value, e => e.Key);
        }
    }
}

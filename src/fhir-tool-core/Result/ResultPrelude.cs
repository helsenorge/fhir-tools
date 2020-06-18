using System;

namespace FhirTool.Core
{
    public static class ResultPrelude
    {
        public static Ok<R> Ok<R>(R value) => new Ok<R>(value);
        public static Fail<T> Fail<T>(Exception error) => new Fail<T>(error);

        // This allows us to use LINQ with the Result<T> type
        public static Result<TB> Select<TA, TB>(this Result<TA> a, Func<TA, TB> select) => a.Bind(aVal => Ok(select(aVal)));

        // This allows us to use LINQ with the Result<T> type
        public static Result<TC> SelectMany<TA, TB, TC>(this Result<TA> a, Func<TA, Result<TB>> func, Func<TA, TB, TC> select) =>
            a.Bind(aVal => func(aVal).Bind(bVal => Ok(select(aVal, bVal))));
    }
}

using System;

namespace FhirTool.Core
{
    public abstract class Result<T>
    {
        public static implicit operator Result<T>(T value) => Ok(value);

        public bool Success => this is Ok<T>;

        public static Ok<R> Ok<R>(R value) => new Ok<R>(value);

        public static Fail<R> Fail<R>(Exception error) => new Fail<R>(error);

        public T ValueOrDefault(T d) => this is Ok<T> ok ? ok.Value : d;

        public T ValueOrElse(Func<Exception, T> f) => this switch
        {
            Ok<T> ok => ok.Value,
            Fail<T> err => f(err.Error),
            _ => default
        };

        //public T ValueOrThrow() => this switch
        //{
        //    Ok<T> ok => ok.Value,
        //    Fail<T> err => throw err.Error,
        //    _ => default
        //};


        public S Handle<S>(Func<T, S> ok, Func<Exception, S> fail) => this switch
        {
            Ok<T> o => ok(o.Value),
            Fail<T> f => fail(f.Error),
            _ => default
        };

        public Result<S> Bind<S>(Func<T, Result<S>> f) => this switch
        {
            Ok<T> ok => f(ok.Value),
            Fail<T> err => Fail<S>(err.Error),
            _ => default
        };
    }
}

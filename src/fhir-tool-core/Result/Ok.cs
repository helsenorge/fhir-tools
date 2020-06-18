using System;

namespace FhirTool.Core
{
    public sealed class Ok<T> : Result<T>
    {
        public T Value { get; private set; }

        public Ok(T value) => Value = value ?? throw new ArgumentNullException(nameof(value));

        public static implicit operator Ok<T>(T value) => new Ok<T>(value);
        public static implicit operator T(Ok<T> value) => value.Value;
    }
}

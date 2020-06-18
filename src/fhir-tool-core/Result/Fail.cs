using System;

namespace FhirTool.Core
{
    public sealed class Fail<T> : Result<T>
    {
        public Exception Error { get; private set; }

        public Fail(Exception error) => Error = error ?? throw new ArgumentNullException(nameof(error));

        public void Throw() => throw Error;
    }
}

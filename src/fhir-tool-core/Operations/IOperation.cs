using System;
using System.Collections.Generic;

namespace FhirTool.Core.Operations
{
    public interface IOperation<T>
    {
        new Result<T> Execute(FhirToolArguments arguments);

        IEnumerable<Issue> Issues { get; }
    }
}

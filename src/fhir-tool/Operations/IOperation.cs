using FhirTool.Core;
using System.Collections.Generic;

namespace FhirTool.Operations
{
    internal interface IOperation
    {
        void Execute(FhirToolArguments arguments);

        IEnumerable<Issue> Issues { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    public interface IOperation
    {
        Task<OperationResultEnum> Execute();

        IEnumerable<Issue> Issues { get; }
    }
}

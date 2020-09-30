using System.Collections.Generic;
using System.Threading.Tasks;

namespace FhirTool.Core.Operations
{
    public abstract class Operation : IOperation
    {
        protected readonly List<Issue> _issues = new List<Issue>();

        public virtual IEnumerable<Issue> Issues => _issues;

        public abstract Task<OperationResultEnum> Execute();
    }
}

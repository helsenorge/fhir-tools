/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

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

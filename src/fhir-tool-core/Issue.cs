/* 
 * Copyright (c) 2021, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

namespace FhirTool.Core
{
    public enum IssueSeverityEnum
    {
        None = 0,
        Information = 1,
        Warning = 2,
        Error = 3
    }

    public class Issue
    {
        public string LinkId { get; set; }
        public IssueSeverityEnum Severity { get; set; }
        public string Details { get; set; }
    }
}
/* 
 * Copyright (c) 2021 - 2023, Norsk Helsenett SF and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the MIT license
 * available at https://raw.githubusercontent.com/helsenorge/fhir-tools/master/LICENSE
 */

using FileHelpers;

namespace FhirTool.Core.Model.FlatFile
{
    [DelimitedRecord("\t")]
    public class ValueSetHeader
    {
        [FieldOptional]
        public string RecordType;
        [FieldOptional]
        public string Id;
        [FieldOptional]
        public string Url;
        [FieldOptional]
        public string Version;
        [FieldOptional]
        public string Name;
        [FieldOptional]
        public string Title;
        [FieldOptional]
        public string Status;
        [FieldOptional]
        public string Date;
        [FieldOptional]
        public string Publisher;
        [FieldOptional]
        public string System;
    }

    [DelimitedRecord("\t")]
    public class ValueSetCodeReferences
    {
        [FieldOptional]
        public string RecordType;
        [FieldOptional]
        public string Code;
        [FieldOptional]
        [FieldQuoted]
        public string Display;
        [FieldOptional]
        public string Reservered0;
        [FieldOptional]
        public string Reservered1;
        [FieldOptional]
        public string Reservered2;
        [FieldOptional]
        public string Reservered3;
        [FieldOptional]
        public string Reservered4;
        [FieldOptional]
        public string Reservered5;
        [FieldOptional]
        public string Reservered6;
    }
}

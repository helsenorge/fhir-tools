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
    public class QuestionnaireHeader
    {
        [FieldOptional]
        public string RecordType;
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
        [FieldQuoted]
        public string Description;
        [FieldOptional]
        public string Purpose;
        [FieldOptional]
        public string UseContext;
        [FieldOptional]
        public string Contact;
        [FieldOptional]
        public string SubjectType;
        [FieldOptional]
        public string Language;
        [FieldOptional]
        public string Id;
        [FieldOptional]
        public string Reserved0;
        [FieldOptional]
        public string Reserved1;
        [FieldOptional]
        public string Reserved2;
        [FieldOptional]
        public string Reserved3;
        [FieldOptional]
        public string Reserved4;
        [FieldOptional]
        public string Reserved5;
    }
    
    [DelimitedRecord("\t")]
    public class QuestionnaireItem
    {
        [FieldOptional]
        public string RecordType;
        [FieldOptional]
        public string Type;
        [FieldOptional]
        public string LinkId;
        [FieldOptional]
        public string Prefix;
        [FieldOptional]
        [FieldQuoted]
        public string Text;
        [FieldOptional]
        public string ValidationText;
        [FieldOptional]
        [FieldNullValue(false)]
        [FieldConverter(ConverterKind.Boolean, "true", "false")]
        public bool? Required;
        [FieldOptional]
        public string CodeValue;
        [FieldOptional]
        public string ReferenceValue;
        [FieldOptional]
        [FieldNullValue(false)]
        [FieldConverter(ConverterKind.Boolean, "true", "false")]
        public bool Repeats;
        [FieldOptional]
        [FieldQuoted]
        public string EnableWhen;
        [FieldOptional]
        [FieldQuoted]
        public string Question;
        [FieldOptional]
        [FieldQuoted]
        public string Answer;
        [FieldOptional]
        [FieldNullValue(false)]
        [FieldConverter(ConverterKind.Boolean, "true", "false")]
        public bool HasAnswer;
        [FieldOptional]
        public string Unit;
        [FieldOptional]
        public string Validation;
        [FieldOptional]
        [FieldNullValue(false)]
        [FieldConverter(ConverterKind.Boolean, "true", "false")]
        public bool ReadOnly;
        [FieldOptional]
        public string Initial;
        [FieldOptional]
        public string EntryFormat;
        [FieldOptional]
        public int? MinValue;
        [FieldOptional]
        public int? MaxValue;
    }
}

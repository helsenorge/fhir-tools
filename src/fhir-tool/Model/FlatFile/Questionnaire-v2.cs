using FileHelpers;
using System;

namespace FhirTool.Model.FlatFile
{
    [DelimitedRecord("\t")]
    public class QuestionnaireHeader2
    {
        [FieldOptional]
        public string RecordType;
        [FieldOptional]
        public string Id;
        [FieldOptional]
        public string Language;
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
        public string ApprovalDate;
        [FieldOptional]
        public string LastReviewDate;
        [FieldOptional]
        public string EffectivePeriod;
        [FieldOptional]
        public string ContactName;
        [FieldOptional]
        public string Copyright;
        [FieldOptional]
        public string SubjectType;
        [FieldOptional]
        [FieldQuoted]
        public string UseContext;
        [FieldOptional]
        public string Endpoint;
        [FieldOptional]
        public string AuthenticationRequirement;
        [FieldOptional]
        public string AccessibilityToResponse;
        [FieldOptional]
        public string CanBePerformedBy;
        [FieldOptional]
        public string Discretion;
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
        [FieldOptional]
        public string Reservered7;
    }

    [DelimitedRecord("\t")]
    public class QuestionnaireItem2
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
        public string Options;
        [FieldOptional]
        [FieldNullValue(false)]
        [FieldConverter(ConverterKind.Boolean, "true", "false")]
        public bool Repeats;
        [FieldOptional]
        [FieldQuoted]
        public string EnableWhen;
        [FieldOptional]
        [FieldNullValue(false)]
        [FieldConverter(ConverterKind.Boolean, "true", "false")]
        public bool ReadOnly;
        [FieldOptional]
        public string Initial;
        [FieldOptional]
        public string EntryFormat;
        [FieldOptional]
        public int? MinValueInteger;
        [FieldOptional]
        public int? MaxValueInteger;
        [FieldOptional]
        public DateTime? MinValueDate;
        [FieldOptional]
        public DateTime? MaxValueDate;
        [FieldOptional]
        public int? MinLength;
        [FieldOptional]
        public int? MaxLength;
        [FieldOptional]
        public int? MaxDecimalPlaces;
        [FieldOptional]
        public string RepeatsText;
        [FieldOptional]
        public string ItemControl;
        [FieldOptional]
        public int? MinOccurs;
        [FieldOptional]
        public int? MaxOccurs;
        [FieldOptional]
        public string Regex;
        [FieldOptional]
        [FieldQuoted]
        public string Markdown;
        [FieldOptional]
        [FieldQuoted]
        public string Unit;
        [FieldOptional]
        [FieldQuoted]
        public string Code;
        [FieldOptional]
        [FieldQuoted]
        public string Option;
        [FieldOptional]
        public string FhirPath;
    }
}

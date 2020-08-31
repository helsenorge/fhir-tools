extern alias R3;

using FhirTool.Core.Operations;
using R3::Hl7.Fhir.Model;
using System.Collections.Generic;

namespace FhirTool.Core
{
    public sealed class FhirToolArguments
    {
        public const string GENERATE_OP = "generate";
        public const string DOWNLOAD_OP = "download";
        public const string UPLOAD_OP = "upload";
        public const string UPLOAD_DEFINITIONS_OP = "upload-definitions";
        public const string BUNDLE_OP = "bundle";
        public const string SPLIT_BUNDLE_OP = "split-bundle";
        public const string TRANSFER_DATA_OP = "transfer-data";
        public const string VERIFY_VALIDATION_OP = "verify-validation";
        public const string CONVERT_OP = "convert";

        public const string QUESTIONNAIRE_ARG = "--questionnaire";
        public const string QUESTIONNAIRE_SHORT_ARG = "-q";
        public const string VALUESET_ARG = "--valueset";
        public const string VALUESET_SHORT_ARG = "-s";
        public const string FHIRBASEURL_ARG = "--fhir-base-url";
        public const string FHIRBASEURL_SHORT_ARG = "-u";
        public const string VERSION_ARG = "--version";
        public const string VERSION_SHORT_ARG = "-v";
        public const string RESOLVEURL_ARG = "--resolve-url";
        public const string RESOLVEURL_SHORT_ARG = "-r";
        public const string VERBOSE_ARG = "--verbose";
        public const string VERBOSE_SHORT_ARG = "-V";
        public const string MIMETYPE_ARG = "--format";
        public const string MIMETYPE_SHORT_ARG = "-f";
        public const string SOURCE_ARG = "--source";
        public const string SOURCE_SHORT_ARG = "-S";
        public const string OUT_ARG = "--out";
        public const string OUT_SHORT_ARG = "-o";
        public const string CREDENTIALS_ARG = "--credentials";
        public const string CREDENTIALS_SHORT_ARG = "-c";
        public const string ENVIRONMENT_ARG = "--environment";
        public const string ENVIRONMENT_SHORT_ARG = "-e";
        public const string CONVERT_FROM_ARG = "--convert-from";
        public const string CONVERT_FROM_SHORT_ARG = "-cf";
        public const string CONVERT_TO_ARG = "--convert-to";
        public const string CONVERT_TO_SHORT_ARG = "-ct";
        public const string DOWNLOAD_RESOURCES_ARG = "--resources";
        public const string FHIR_VERSION = "--fhir-version";
        public const string KEEP_SERVER_URL = "--keep-server-url";

        public const string ENVIRONMENT_SOURCE_ARG = "--environment-source";
        public const string ENVIRONMENT_SOURCE_SHORT_ARG = "-es";
        public const string ENVIRONMENT_DESTINATION_ARG = "--environment-destination";
        public const string ENVIRONMENT_DESTINATION_SHORT_ARG = "-ed";
        public const string RESOURCETYPE_ARG = "--resourcetype";
        public const string RESOURCETYPE_SHORT_ARG = "-R";

        public const string SKIP_VALIDATION_ARG = "--skip-validation";
        public const string SKIP_VALIDATION_SHORT_ARG = "-sv";

        public const string SEARCHCOUNT_ARG = "--searchcount";
        public const string SEARCHCOUNT_SHORT_ARG = "-sc";

        // TODO: When C#9 use init accessor or use record in place of class
        public OperationEnum Operation { get; set; }
        public string QuestionnairePath { get; set; }
        public string ValueSetPath { get; set; }
        public string FhirBaseUrl { get; set; }
        public string ProxyBaseUrl { get; set; }
        public string SourceFhirBaseUrl { get; set; }
        public string SourceProxyBaseUrl { get; set; }
        public string DestinationFhirBaseUrl { get; set; }
        public string DestinationProxyBaseUrl { get; set; }
        public bool ResolveUrl { get; set; }
        public string Version { get; set; }
        public bool Verbose { get; set; }
        public string MimeType { get; set; }
        public string SourcePath { get; set; }
        public string OutPath { get; set; }
        public string Credentials { get; set; }
        public string Environment { get; set; }
        public string SourceEnvironment { get; set; }
        public string DestinationEnvironment { get; set; }
        public ResourceType? ResourceType { get; set; }
        public int SearchCount { get; set; }
        public bool SkipValidation { get; set; }
        public string FromFhirVersion { get; set; }
        public string ToFhirVersion { get; set; }
        public List<string> Resources { get; set; }
        public FhirVersion? FhirVersion { get; set; }
        public bool KeepServerUrl { get; set; }
    }
}

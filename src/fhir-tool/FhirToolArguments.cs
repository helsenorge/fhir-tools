using FhirTool.Configuration;
using System.Linq;
using System.Configuration;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;

namespace FhirTool
{
    public enum OperationEnum
    {
        None = 0,
        Generate = 1,
        Upload = 2,
        UploadDefinitions = 3,
        Bundle = 4,
        SplitBundle = 5,
        TransferData = 6,
        VerifyValidation = 7
    }

    public sealed class FhirToolArguments
    {
        public static readonly string[] SUPPORTED_MIMETYPES = { "xml", "json" };

        public const string GENERATE_OP = "generate";
        public const string UPLOAD_OP = "upload";
        public const string UPLOAD_DEFINITIONS_OP = "upload-definitions";
        public const string BUNDLE_OP = "bundle";
        public const string SPLIT_BUNDLE_OP = "split-bundle";
        public const string TRANSFER_DATA_OP = "transfer-data";
        public const string VERIFY_VALIDATION_OP = "verify-validation";

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

        public const string ENVIRONMENT_SOURCE_ARG = "--environment-source";
        public const string ENVIRONMENT_SOURCE_SHORT_ARG = "-es";
        public const string ENVIRONMENT_DESTINATION_ARG = "--environment-destination";
        public const string ENVIRONMENT_DESTINATION_SHORT_ARG = "-ed";
        public const string RESOURCETYPE_ARG = "--resourcetype";
        public const string RESOURCETYPE_SHORT_ARG = "-R";

        public const string SEARCHCOUNT_ARG = "--searchcount";
        public const string SEARCHCOUNT_SHORT_ARG = "-sc";

        public OperationEnum Operation { get; internal set; }
        public string QuestionnairePath { get; internal set; }
        public string ValueSetPath { get; internal set; }
        public string FhirBaseUrl { get; internal set; }
        public string ProxyBaseUrl { get; internal set; }
        public bool ResolveUrl { get; internal set; }
        public string Version { get; internal set; }
        public bool Verbose { get; internal set; }
        public string MimeType { get; internal set; }
        public string SourcePath { get; internal set; }
        public string OutPath { get; internal set; }
        public string Credentials { get; internal set; }
        public string Environment { get; set; }
        public string SourceEnvironment { get; set; }
        public string DestinationEnvironment { get; set; }
        public ResourceType? ResourceType { get; set; }
        public int SearchCount { get; set; }

        public static FhirToolArguments Create(string[] args)
        {
            FhirToolArguments arguments = new FhirToolArguments();

            for(int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch(arg)
                {
                    case GENERATE_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.Generate;
                        break;
                    case UPLOAD_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.Upload;
                        break;
                    case UPLOAD_DEFINITIONS_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.UploadDefinitions;
                        break;
                    case BUNDLE_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.Bundle;
                        break;
                    case SPLIT_BUNDLE_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.SplitBundle;
                        break;
                    case TRANSFER_DATA_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.TransferData;
                        break;
                    case VERIFY_VALIDATION_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.VerifyValidation;
                        break;
                    case QUESTIONNAIRE_ARG:
                    case QUESTIONNAIRE_SHORT_ARG:
                        arguments.QuestionnairePath = args[i + 1];
                        break;
                    case VALUESET_ARG:
                    case VALUESET_SHORT_ARG:
                        arguments.ValueSetPath = args[i + 1];
                        break;
                    case FHIRBASEURL_ARG:
                    case FHIRBASEURL_SHORT_ARG:
                        arguments.FhirBaseUrl = args[i + 1];
                        break;
                    case VERSION_ARG:
                    case VERSION_SHORT_ARG:
                        arguments.Version = args[i + 1];
                        break;
                    case RESOLVEURL_ARG:
                    case RESOLVEURL_SHORT_ARG:
                        arguments.ResolveUrl = true;
                        break;
                    case VERBOSE_ARG:
                    case VERBOSE_SHORT_ARG:
                        arguments.Verbose = true;
                        break;
                    case MIMETYPE_ARG:
                    case MIMETYPE_SHORT_ARG:
                        string mimeType = args[i + 1].ToLowerInvariant();
                        if (!SUPPORTED_MIMETYPES.Contains(mimeType)) throw new NotSupportedMimeTypeException(mimeType);
                        arguments.MimeType = mimeType;
                        break;
                    case SOURCE_ARG:
                    case SOURCE_SHORT_ARG:
                        arguments.SourcePath = args[i + 1];
                        break;
                    case OUT_ARG:
                    case OUT_SHORT_ARG:
                        arguments.OutPath = args[i + 1];
                        break;
                    case CREDENTIALS_ARG:
                    case CREDENTIALS_SHORT_ARG:
                        arguments.Credentials = args[i + 1];
                        break;
                    case ENVIRONMENT_ARG:
                    case ENVIRONMENT_SHORT_ARG:
                        arguments.Environment = args[i + 1];
                        EnvironmentSection environmentSection = (EnvironmentSection)ConfigurationManager.GetSection($"environmentSection");
                        EnvironmentElement environment = (EnvironmentElement)environmentSection.Items[arguments.Environment];
                        arguments.FhirBaseUrl = environment.FhirBaseUrl;
                        arguments.ProxyBaseUrl = environment.ProxyBaseUrl;
                        break;
                    case ENVIRONMENT_SOURCE_ARG:
                    case ENVIRONMENT_SOURCE_SHORT_ARG:
                        arguments.SourceEnvironment = args[i + 1];
                        break;
                    case ENVIRONMENT_DESTINATION_ARG:
                    case ENVIRONMENT_DESTINATION_SHORT_ARG:
                        arguments.DestinationEnvironment = args[i + 1];
                        break;
                    case RESOURCETYPE_ARG:
                    case RESOURCETYPE_SHORT_ARG:
                        arguments.ResourceType = EnumUtility.ParseLiteral<ResourceType>(args[i + 1]);
                        break;
                    case SEARCHCOUNT_ARG:
                    case SEARCHCOUNT_SHORT_ARG:
                        int searchCount;
                        if (!int.TryParse(args[i + 1], out searchCount))
                            throw new RequiredArgumentException($"{SEARCHCOUNT_ARG}|{SEARCHCOUNT_SHORT_ARG}");
                        arguments.SearchCount = searchCount;
                        break;
                    default:
                        break;
                }
            }

            return arguments;
        }

        private static EnvironmentSection _environmentSection;
        private static EnvironmentSection GetEnvironmentSection()
        {
            if(_environmentSection == null) _environmentSection = (EnvironmentSection)ConfigurationManager.GetSection($"environmentSection");
            return _environmentSection;
        }

        public static EnvironmentElement GetEnvironmentElement(string name)
        {
            EnvironmentSection environmentSection = GetEnvironmentSection();
            if (!environmentSection.Items.Exists(name)) return null;
            EnvironmentElement environment = environmentSection.Items[name] as EnvironmentElement;

            return environment;
        }

        public static bool IsKnownEnvironment(string name)
        {
            return GetEnvironmentElement(name) != null;
        }
    }
}
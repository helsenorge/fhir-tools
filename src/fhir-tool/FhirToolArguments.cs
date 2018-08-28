using System.Linq;

namespace FhirTool
{
    public enum OperationEnum
    {
        None = 0,
        Generate = 1,
        Upload = 2,
        UploadDefinitions = 3,
        Bundle = 4,
        SplitBundle = 5
    }

    public sealed class FhirToolArguments
    {
        public static readonly string[] SUPPORTED_MIMETYPES = { "xml", "json" };

        public const string GENERATE_OP = "generate";
        public const string UPLOAD_OP = "upload";
        public const string UPLOAD_DEFINITIONS_OP = "upload-definitions";
        public const string BUNDLE_OP = "bundle";
        public const string SPLIT_BUNDLE_OP = "split-bundle";

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

        public OperationEnum Operation { get; private set; }
        public string QuestionnairePath { get; private set; }
        public string ValueSetPath { get; private set; }
        public string FhirBaseUrl { get; private set; }
        public bool ResolveUrl { get; private set; }
        public string Version { get; private set; }
        public bool Verbose { get; private set; }
        public string MimeType { get; private set; }
        public string SourcePath { get; private set; }
        public string OutPath { get; private set; }
        public string Credentials { get; private set; }

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
                    default:
                        break;
                }
            }

            return arguments;
        }
    }
}
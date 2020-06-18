using FhirTool.Configuration;
using System.Linq;
using System.Configuration;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using EnsureThat;
using FhirTool.Core;

namespace FhirTool
{
    public sealed class HandleFhirToolArguments
    {
        public static readonly string[] SUPPORTED_MIMETYPES = { "xml", "json" };

        public static FhirToolArguments Create(string[] args)
        {
            EnsureArg.IsNotNull(args, nameof(args));

            FhirToolArguments arguments = new FhirToolArguments();

            for(int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch(arg)
                {
                    case FhirToolArguments.GENERATE_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.Generate;
                        break;
                    case FhirToolArguments.UPLOAD_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.Upload;
                        break;
                    case FhirToolArguments.UPLOAD_DEFINITIONS_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.UploadDefinitions;
                        break;
                    case FhirToolArguments.BUNDLE_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.Bundle;
                        break;
                    case FhirToolArguments.SPLIT_BUNDLE_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.SplitBundle;
                        break;
                    case FhirToolArguments.TRANSFER_DATA_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.TransferData;
                        break;
                    case FhirToolArguments.VERIFY_VALIDATION_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.VerifyValidation;
                        break;
                    case FhirToolArguments.CONVERT_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.Convert;
                        break;
                    case FhirToolArguments.QUESTIONNAIRE_ARG:
                    case FhirToolArguments.QUESTIONNAIRE_SHORT_ARG:
                        arguments.QuestionnairePath = args[i + 1];
                        break;
                    case FhirToolArguments.VALUESET_ARG:
                    case FhirToolArguments.VALUESET_SHORT_ARG:
                        arguments.ValueSetPath = args[i + 1];
                        break;
                    case FhirToolArguments.FHIRBASEURL_ARG:
                    case FhirToolArguments.FHIRBASEURL_SHORT_ARG:
                        arguments.FhirBaseUrl = args[i + 1];
                        break;
                    case FhirToolArguments.VERSION_ARG:
                    case FhirToolArguments.VERSION_SHORT_ARG:
                        arguments.Version = args[i + 1];
                        break;
                    case FhirToolArguments.RESOLVEURL_ARG:
                    case FhirToolArguments.RESOLVEURL_SHORT_ARG:
                        arguments.ResolveUrl = true;
                        break;
                    case FhirToolArguments.VERBOSE_ARG:
                    case FhirToolArguments.VERBOSE_SHORT_ARG:
                        arguments.Verbose = true;
                        break;
                    case FhirToolArguments.MIMETYPE_ARG:
                    case FhirToolArguments.MIMETYPE_SHORT_ARG:
                        string mimeType = args[i + 1].ToLowerInvariant();
                        if (!SUPPORTED_MIMETYPES.Contains(mimeType)) throw new NotSupportedMimeTypeException(mimeType);
                        arguments.MimeType = mimeType;
                        break;
                    case FhirToolArguments.SOURCE_ARG:
                    case FhirToolArguments.SOURCE_SHORT_ARG:
                        arguments.SourcePath = args[i + 1];
                        break;
                    case FhirToolArguments.OUT_ARG:
                    case FhirToolArguments.OUT_SHORT_ARG:
                        arguments.OutPath = args[i + 1];
                        break;
                    case FhirToolArguments.CREDENTIALS_ARG:
                    case FhirToolArguments.CREDENTIALS_SHORT_ARG:
                        arguments.Credentials = args[i + 1];
                        break;
                    case FhirToolArguments.ENVIRONMENT_ARG:
                    case FhirToolArguments.ENVIRONMENT_SHORT_ARG:
                        arguments.Environment = args[i + 1];
                        EnvironmentSection environmentSection = (EnvironmentSection)ConfigurationManager.GetSection($"environmentSection");
                        EnvironmentElement environment = (EnvironmentElement)environmentSection.Items[arguments.Environment];
                        arguments.FhirBaseUrl = environment.FhirBaseUrl;
                        arguments.ProxyBaseUrl = environment.ProxyBaseUrl;
                        break;
                    case FhirToolArguments.ENVIRONMENT_SOURCE_ARG:
                    case FhirToolArguments.ENVIRONMENT_SOURCE_SHORT_ARG:
                        arguments.SourceEnvironment = args[i + 1];
                        break;
                    case FhirToolArguments.ENVIRONMENT_DESTINATION_ARG:
                    case FhirToolArguments.ENVIRONMENT_DESTINATION_SHORT_ARG:
                        arguments.DestinationEnvironment = args[i + 1];
                        break;
                    case FhirToolArguments.RESOURCETYPE_ARG:
                    case FhirToolArguments.RESOURCETYPE_SHORT_ARG:
                        arguments.ResourceType = EnumUtility.ParseLiteral<ResourceType>(args[i + 1]);
                        break;
                    case FhirToolArguments.SEARCHCOUNT_ARG:
                    case FhirToolArguments.SEARCHCOUNT_SHORT_ARG:
                        int searchCount;
                        if (!int.TryParse(args[i + 1], out searchCount))
                            throw new RequiredArgumentException($"{FhirToolArguments.SEARCHCOUNT_ARG}|{FhirToolArguments.SEARCHCOUNT_SHORT_ARG}");
                        arguments.SearchCount = searchCount;
                        break;
                    case FhirToolArguments.SKIP_VALIDATION_ARG:
                    case FhirToolArguments.SKIP_VALIDATION_SHORT_ARG:
                        arguments.SkipValidation = true;
                        break;
                    case FhirToolArguments.CONVERT_FROM_ARG:
                    case FhirToolArguments.CONVERT_FROM_SHORT_ARG:
                        arguments.FromFhirVersion = FhirVersionInternal.ConvertToFhirVersion(args[i + 1]);
                        break;
                    case FhirToolArguments.CONVERT_TO_ARG:
                    case FhirToolArguments.CONVERT_TO_SHORT_ARG:
                        arguments.ToFhirVersion = FhirVersionInternal.ConvertToFhirVersion(args[i + 1]);
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
            EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

            EnvironmentSection environmentSection = GetEnvironmentSection();
            if (!environmentSection.Items.Exists(name)) return null;
            EnvironmentElement environment = environmentSection.Items[name] as EnvironmentElement;

            return environment;
        }

        public static bool IsKnownEnvironment(string name)
        {
            EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

            return GetEnvironmentElement(name) != null;
        }
    }
}
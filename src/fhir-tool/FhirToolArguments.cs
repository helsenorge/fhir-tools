extern alias R3;

using FhirTool.Configuration;
using System.Linq;
using System.Configuration;
using R3::Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using EnsureThat;
using FhirTool.Core;
using FhirTool.Core.Operations;
using FhirTool.Core.Utils;

namespace FhirTool
{
    public sealed class HandleFhirToolArguments
    {
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
                        arguments.Operation = OperationEnum.GenerateResource;
                        break;
                    case FhirToolArguments.DOWNLOAD_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.DownloadResources;
                        break;
                    case FhirToolArguments.UPLOAD_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.UploadResource;
                        break;
                    case FhirToolArguments.UPLOAD_DEFINITIONS_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.UploadFhirDefinitions;
                        break;
                    case FhirToolArguments.BUNDLE_OP:
                        if (arguments.Operation != OperationEnum.None) throw new MultipleOperationException(arguments.Operation);
                        arguments.Operation = OperationEnum.BundleResources;
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
                        if (!Constants.SUPPORTED_MIMETYPES.Contains(mimeType)) throw new NotSupportedMimeTypeException(mimeType);
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
                        if (!IsKnownEnvironment(arguments.Environment)) throw new UnknownEnvironmentNameException(arguments.Environment);
                        EnvironmentElement environment = GetEnvironmentElement(arguments.Environment);
                        arguments.FhirBaseUrl = environment.FhirBaseUrl;
                        arguments.ProxyBaseUrl = environment.ProxyBaseUrl;
                        break;
                    case FhirToolArguments.ENVIRONMENT_SOURCE_ARG:
                    case FhirToolArguments.ENVIRONMENT_SOURCE_SHORT_ARG:
                        arguments.SourceEnvironment = args[i + 1];
                        if (!IsKnownEnvironment(arguments.SourceEnvironment)) throw new UnknownEnvironmentNameException(arguments.SourceEnvironment);

                        EnvironmentElement srcEnv = GetEnvironmentElement(arguments.Environment);
                        arguments.SourceFhirBaseUrl = srcEnv.FhirBaseUrl;
                        arguments.SourceProxyBaseUrl = srcEnv.ProxyBaseUrl;
                        break;
                    case FhirToolArguments.ENVIRONMENT_DESTINATION_ARG:
                    case FhirToolArguments.ENVIRONMENT_DESTINATION_SHORT_ARG:
                        arguments.DestinationEnvironment = args[i + 1];
                        if (!IsKnownEnvironment(arguments.DestinationEnvironment)) throw new UnknownEnvironmentNameException(arguments.DestinationEnvironment);

                        EnvironmentElement destEnv = GetEnvironmentElement(arguments.Environment);
                        arguments.DestinationFhirBaseUrl = destEnv.FhirBaseUrl;
                        arguments.DestinationProxyBaseUrl = destEnv.ProxyBaseUrl;
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
                    case FhirToolArguments.DOWNLOAD_RESOURCES_ARG:
                        arguments.Resources = args[i + 1].Split(",").ToList();
                        break;
                    case FhirToolArguments.FHIR_VERSION:
                        arguments.FhirVersion = FhirVersionUtils.MapStringToFhirVersion(args[i + 1]);
                        break;
                    case FhirToolArguments.KEEP_SERVER_URL:
                        arguments.KeepServerUrl = true;
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
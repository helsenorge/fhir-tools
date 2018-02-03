namespace FhirTool
{
    public class FhirToolArguments
    {
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

        public string QuestionnairePath { get; set; }
        public string ValueSetPath { get; set; }
        public string FhirBaseUrl { get; set; }
        public bool ResolveUrl { get; set; }
        public string Version { get; set; }
        public bool Verbose { get; set; }

        public static FhirToolArguments Create(string[] args)
        {
            FhirToolArguments arguments = new FhirToolArguments();

            for(int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch(arg)
                {
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
                    default:
                        break;
                }
            }

            return arguments;
        }
    }
}
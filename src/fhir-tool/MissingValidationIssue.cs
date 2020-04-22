namespace FhirTool
{
    public enum MissingValidationSeverityEnum
    {
        None = 0,
        Information = 1,
        Warning = 2,
        Error = 3
    }

    public class MissingValidationIssue
    {
        public string LinkId { get; internal set; }
        public MissingValidationSeverityEnum Severity { get; internal set; }
        public string Details { get; internal set; }
    }
}
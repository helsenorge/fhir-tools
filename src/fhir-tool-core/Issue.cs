namespace FhirTool.Core
{
    public enum IssueSeverityEnum
    {
        None = 0,
        Information = 1,
        Warning = 2,
        Error = 3
    }

    public class Issue
    {
        public string LinkId { get; set; }
        public IssueSeverityEnum Severity { get; set; }
        public string Details { get; set; }
    }
}
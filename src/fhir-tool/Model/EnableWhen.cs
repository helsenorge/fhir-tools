namespace FhirTool.Model
{
    public class EnableWhen
    {
        public string Question { get; set; }
        public bool? HasAnswer { get; set; }
        public bool? AnswerBoolean { get; set; }
        public decimal? AnswerDecimal { get; set; }
        public int? AnswerInteger { get; set; }
        public string AnswerDate { get; set; }
        public string AnswerDateTime { get; set; }
        public string AnswerTime { get; set; }
        public string AnswerString { get; set; }
        public string AnswerUri { get; set; }
        public EnableWhenReference AnswerReference { get; set; }
        public EnableWhenCoding AnswerCoding { get; set; }
        public EnableWhenQuantity AnswerQuantity { get; set; }
    }
}
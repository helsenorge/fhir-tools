namespace FhirTool.Core.Model
{
    public class EnableWhenElement
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
        public ReferenceElement AnswerReference { get; set; }
        public CodingElement AnswerCoding { get; set; }
        public QuantityElement AnswerQuantity { get; set; }

        public object Answer
        {
            get
            {
                if (AnswerBoolean.HasValue) return AnswerBoolean.Value;
                if (AnswerDecimal.HasValue) return AnswerDecimal.Value;
                if (AnswerInteger.HasValue) return AnswerInteger.Value;
                if (!string.IsNullOrWhiteSpace(AnswerDate)) return AnswerDate;
                if (!string.IsNullOrWhiteSpace(AnswerDateTime)) return AnswerDateTime;
                if (!string.IsNullOrWhiteSpace(AnswerTime)) return AnswerTime;
                if (!string.IsNullOrWhiteSpace(AnswerUri)) return AnswerUri;
                if (AnswerReference == null) return AnswerReference;
                if (AnswerCoding == null) return AnswerCoding;
                if (AnswerQuantity == null) return AnswerQuantity;

                return null;
            }
        }
    }
}
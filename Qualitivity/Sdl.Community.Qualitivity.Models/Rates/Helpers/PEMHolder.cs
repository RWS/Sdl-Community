namespace Structures.Rates.Helpers
{
    public class PemHolder
    {
        public string SegmentId { get; set; }
        public decimal CharsDistPer { get; set; }
        public decimal TagsDistPerRem { get; set; }
        public decimal CharsTotal { get; set; }

        public PemHolder()
        {
            SegmentId = string.Empty;
            CharsDistPer = 0;
            TagsDistPerRem = 0;
            CharsTotal = 0;
        }
        public PemHolder(string segmentId, decimal charsDistPer, decimal tagsDistPerRem, decimal charsTotal)
        {
            SegmentId = segmentId;
            CharsDistPer = charsDistPer;
            TagsDistPerRem = tagsDistPerRem;
            CharsTotal = charsTotal;
        }

    }
}

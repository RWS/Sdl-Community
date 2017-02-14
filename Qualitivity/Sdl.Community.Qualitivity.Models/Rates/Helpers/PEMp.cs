namespace Sdl.Community.Structures.Rates.Helpers
{
    public class PEMp
    {
        public string SegmentId { get; set; }
        public decimal EditDistance { get; set; }
        public decimal ModificationPercentage { get; set; }
        public decimal MaxCharacters { get; set; }

        public PEMp()
        {
            SegmentId = string.Empty;
            EditDistance = 0;
            ModificationPercentage = 0;
            MaxCharacters = 0;
        }
        public PEMp(string segmentId, decimal editDistance, decimal modificationPercentage, decimal maxCharacters)
        {
            SegmentId = segmentId;
            EditDistance = editDistance;
            ModificationPercentage = modificationPercentage;
            MaxCharacters = maxCharacters;
        }
    }
}

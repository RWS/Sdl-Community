namespace Sdl.Community.DsiViewer.Services
{
    /// <summary>
    /// Maps a numeric TQE score (the "tqe-score-N" metadata) to the Good / Adequate / Poor label shown in the panel's
    /// quality badge. Language Weaver writes 80 / 66 / 33 for Good / Adequate / Poor.
    /// </summary>
    public static class QualityEstimationLabel
    {
        public const int GoodThreshold = 67;
        public const int AdequateThreshold = 34;

        public static string FromScore(string score)
        {
            if (!int.TryParse(score, out var numericScore))
            {
                return null;
            }

            return numericScore switch
            {
                >= GoodThreshold => Constants.GoodQuality,
                >= AdequateThreshold => Constants.AdequateQuality,
                _ => Constants.PoorQuality
            };
        }
    }
}

namespace LanguageWeaverProvider.Services.Model
{
    public class TranslationResult
    {
        public string Translation { get; set; }

        /// <summary>Quality estimation label (e.g. "Good", "Adequate", "Poor"), or null if not available.</summary>
        public string QualityEstimation { get; set; }
    }
}

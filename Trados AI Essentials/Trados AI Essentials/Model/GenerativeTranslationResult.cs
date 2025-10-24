using System.Collections.Generic;

namespace Trados_AI_Essentials.Model
{
    public class GenerativeTranslationResult
    {
        public LanguagePairModel LanguagePairModel { get; set; }
        public string Model { get; set; }
        public string Translation { get; set; }
        public List<UsedTranslationResource> UsedTranslationResources { get; set; }
    }
}
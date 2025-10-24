using System.Collections.Generic;

namespace Trados_AI_Essentials.Model
{
    public class TerminologyResource
    {
        public string OriginResource { get; set; }
        public string SourceTerm { get; set; }
        public List<TermTranslation> TermTranslations { get; set; }
    }
}
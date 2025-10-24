using System.Collections.Generic;

namespace Trados_AI_Essentials.Model
{
    public class UsedTranslationResource
    {
        public string ResourceType { get; set; }
        public List<TerminologyResource> TerminologyResources { get; set; }
    }
}
using Newtonsoft.Json;
using Trados_AI_Essentials.Interface;

namespace Trados_AI_Essentials.Model
{
    public class TranslationRequest 
    {
        [JsonIgnore]
        public bool IncludeUserResources { get; set; }
        public string Source { get; set; }
        public string SourceLanguage { get; set; } = string.Empty;
        public string TargetLanguage { get; set; } = string.Empty;

        public string TranslationProfileId { get; set; } = string.Empty;
        public string UserPrompt { get; set; } = string.Empty;
    }
}
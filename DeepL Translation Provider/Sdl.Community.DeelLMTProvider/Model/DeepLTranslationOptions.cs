using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class DeepLTranslationOptions
    {
        [JsonIgnore]
        public string ApiKey { get; set; }

        public string ApiVersion { get; set; }
        public List<string> IgnoreTagsParameter { get; set; }
        public List<LanguagePairOptions> LanguagePairOptions { get; set; }
        public Dictionary<string, string> LanguagesSupported { get; set; } = new();


        public bool PreserveFormatting { get; set; }

        public bool ResendDraft { get; set; }

        public bool SendPlainText { get; set; }
        public SplitSentences SplitSentenceHandling { get; set; } = SplitSentences.Default;

        public string StyleId { get; set; }
        public TagFormat TagHandling { get; set; }
        public Uri Uri { get; set; }
    }
}
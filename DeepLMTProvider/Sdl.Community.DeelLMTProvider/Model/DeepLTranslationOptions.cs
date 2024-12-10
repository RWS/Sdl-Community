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

        [JsonIgnore]
        public bool PreserveFormatting
        {
            get => PreserveFormattingParameter != null && Convert.ToBoolean(PreserveFormattingParameter);
            set => PreserveFormattingParameter = value.ToString();
        }

        public string PreserveFormattingParameter { get; set; }

        [JsonIgnore]
        public bool SendPlainText
        {
            get => SendPlainTextParameter != null && Convert.ToBoolean(SendPlainTextParameter);
            set => SendPlainTextParameter = value.ToString();
        }

        public string SendPlainTextParameter { get; set; }

        [JsonIgnore]
        public TagFormat TagHandling
        {
            get => Enum.TryParse<TagFormat>(TagHandlingParameter, out var tagHandling) ? tagHandling : TagFormat.None;
            set => TagHandlingParameter = value.ToString();
        }

        public string TagHandlingParameter { get; set; }
        public Uri Uri { get; set; }
    }
}
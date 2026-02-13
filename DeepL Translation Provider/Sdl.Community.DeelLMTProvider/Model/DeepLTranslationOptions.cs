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

        [JsonIgnore]
        public bool ResendDraft 
        {
            get => ResendDraftParameter != null && Convert.ToBoolean(ResendDraftParameter);
            set => ResendDraftParameter = value.ToString(); 
        }

        public string SendPlainTextParameter { get; set; }

        public string ResendDraftParameter { get; set; }

        public string ModelTypeParameter { get; set; }

        [JsonIgnore]
        public TagFormat TagHandling
        {
            get => Enum.TryParse<TagFormat>(TagHandlingParameter, out var tagHandling) ? tagHandling : TagFormat.None;
            set => TagHandlingParameter = value.ToString();
        }
        
        [JsonIgnore]
        public ModelType ModelType
        {
            get => Enum.TryParse<ModelType>(ModelTypeParameter, out var modelType) ? modelType : ModelType.Latency_Optimized;
            set => ModelTypeParameter = value.ToString();
        }

        [JsonIgnore]
        public SplitSentences SplitSentencesHandling 
        {
            get => Enum.TryParse<SplitSentences>(SplitSentenceHandlingParameter, out var splitSentencesHandling) ? splitSentencesHandling : SplitSentences.Default;
            set => SplitSentenceHandlingParameter = value.ToString(); 
        }

        public string TagHandlingParameter { get; set; }

        public string SplitSentenceHandlingParameter { get; set; }

        public Uri Uri { get; set; }
        public string StyleId { get; set; }
    }
}
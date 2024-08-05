using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class DeepLTranslationOptions
    {
        private readonly TranslationProviderUriBuilder _uriBuilder;

        public DeepLTranslationOptions()
        {
            _uriBuilder = new TranslationProviderUriBuilder("deepltranslationprovider");
        }

        public DeepLTranslationOptions(Uri uri, string state = null)
        {
            _uriBuilder = new TranslationProviderUriBuilder(uri);

            if (string.IsNullOrWhiteSpace(state))
                return;

            var successful = TryParseJson(state, out var stateObject);
            if (!successful)
                return;

            LanguagesSupported = JsonConvert
                .DeserializeObject<Dictionary<string, string>>(stateObject?["LanguagesSupported"]?.ToString());

            SendPlainTextParameter = stateObject?[nameof(SendPlainTextParameter)]?.ToString();
            PreserveFormattingParameter = stateObject?[nameof(PreserveFormattingParameter)]?.ToString();
            TagHandlingParameter = stateObject?[nameof(TagHandlingParameter)]?.ToString();
            ApiVersionParameter = stateObject?[nameof(ApiVersionParameter)]?.ToString();

            LanguagePairOptions =
                JsonConvert.DeserializeObject<List<LanguagePairOptions>>(stateObject?["LanguagePairOptions"]?.ToString());
        }

        [JsonIgnore]
        public string ApiKey { get; set; }

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

        [JsonIgnore]
        public Uri Uri => _uriBuilder.Uri;

        public string ApiVersionParameter { get; set; }

        [JsonIgnore]
        public ApiVersion ApiVersion 
        {
            get => Enum.TryParse<ApiVersion>(ApiVersionParameter, out var apiVersion) ? apiVersion : ApiVersion.V1;
            set => ApiVersionParameter = value.ToString();
        }

        private bool TryParseJson(string state, out JObject jObject)
        {
            bool successful;
            try
            {
                jObject = JObject.Parse(state);
                successful = true;
            }
            catch
            {
                successful = false;
                jObject = null;
            }

            return successful;
        }
    }
}
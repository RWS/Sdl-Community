using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.LanguagePlatform.TranslationMemoryApi;

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

            if (string.IsNullOrWhiteSpace(state)) return;

            var successful = TryParseJson(state, out var stateObject);
            if (successful)
            {
                LanguagesSupported = JsonConvert
                    .DeserializeObject<Dictionary<string, string>>(stateObject?["LanguagesSupported"]?.ToString());
				SendPlainTextParameter = stateObject?["SendPlainTextParameter"]?.ToString();
				if (Enum.TryParse<Formality>(stateObject["Formality"]?.ToString(), out var formality))
				{
					Formality = formality;
				}
			}
        }

        [JsonIgnore]
        public string ApiKey { get; set; }

        public Formality Formality { get; set; }

        [JsonIgnore]
        public string Identifier { get; set; }

        public Dictionary<string, string> LanguagesSupported { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public bool SendPlainText
        {
            get => SendPlainTextParameter != null && Convert.ToBoolean(SendPlainTextParameter);
            set => SendPlainTextParameter = value.ToString();
        }

        public string SendPlainTextParameter { get; set; } 

        [JsonIgnore]
        public Uri Uri => _uriBuilder.Uri;

        private string GetStringParameter(string p)
        {
            var paramString = _uriBuilder[p];
            return paramString;
        }

        private void SetStringParameter(string p, string value)
        {
            _uriBuilder[p] = value;
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
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

			if (string.IsNullOrWhiteSpace(state))
				return;

			var successful = TryParseJson(state, out var stateObject);
			if (!successful)
				return;

			LanguagesSupported = JsonConvert
				.DeserializeObject<Dictionary<string, string>>(stateObject?["LanguagesSupported"]?.ToString());

			SendPlainTextParameter = stateObject?["SendPlainTextParameter"]?.ToString();

            if (bool.TryParse(stateObject?[nameof(RemoveLockedContent)]?.ToString(), out var removeLockedContent))
                RemoveLockedContent = removeLockedContent;
            
            if (bool.TryParse(stateObject?[nameof(DecodeFromHtmlOrUrl)]?.ToString(), out var decode))
                DecodeFromHtmlOrUrl = decode;

			LanguagePairOptions =
				JsonConvert.DeserializeObject<List<LanguagePairOptions>>(stateObject?["LanguagePairOptions"]?.ToString());
		}

		[JsonIgnore]
		public string ApiKey { get; set; }

		public List<LanguagePairOptions> LanguagePairOptions { get; set; }
		public Dictionary<string, string> LanguagesSupported { get; set; } = new();

		[JsonIgnore]
		public bool SendPlainText
		{
			get => SendPlainTextParameter != null && Convert.ToBoolean(SendPlainTextParameter);
			set => SendPlainTextParameter = value.ToString();
		}

		public string SendPlainTextParameter { get; set; }

		[JsonIgnore]
		public Uri Uri => _uriBuilder.Uri;

        public bool RemoveLockedContent { get; set; }
        public bool DecodeFromHtmlOrUrl { get; set; }

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
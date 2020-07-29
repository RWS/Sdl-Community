using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DeepLMTProvider.WPF.Model
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
			}
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

		[JsonIgnore]
		public string ApiKey { get; set; }

		[JsonIgnore]
		public Formality Formality
		{
			get
			{
				Enum.TryParse<Formality>(GetStringParameter("formality"), out var formality);
				return formality;
			}
			set => SetStringParameter("formality", value.ToString());
		}
		
		[JsonIgnore]
		public string Identifier { get; set; }

		public Dictionary<string, string> LanguagesSupported { get; set; } = new Dictionary<string, string>();

		[JsonIgnore]
		public bool SendPlainText
		{
			get => SendPlainTextParameter != null && Convert.ToBoolean(SendPlainTextParameter);
			set => SendPlainTextParameter = value.ToString();
		}

		[JsonIgnore]
		public string SendPlainTextParameter
		{
			get => GetStringParameter("sendPlain");
			set => SetStringParameter("sendPlain", value);
		}

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
	}
}
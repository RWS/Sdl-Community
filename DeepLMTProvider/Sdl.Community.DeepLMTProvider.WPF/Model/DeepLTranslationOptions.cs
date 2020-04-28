using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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

		public DeepLTranslationOptions(Uri uri)
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}

		[JsonIgnore]
		public string ApiKey { get; set; }

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
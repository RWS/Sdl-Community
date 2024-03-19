using System;
using System.Collections.Generic;
using MicrosoftTranslatorProvider.Interfaces;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MicrosoftTranslatorProvider.Model
{
	public class TranslationOptions : ITranslationOptions
	{
		public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;
		[JsonIgnore]
		public string ApiKey { get; set; }
		public bool PersistMicrosoftCredentials { get; set; }
		public bool SendPlainTextOnly { get; set; }
		public bool ResendDrafts { get; set; }
		public bool UsePreEdit { get; set; }
		public bool UsePostEdit { get; set; }
		public string CategoryID { get; set; }
		public string PreLookupFilename { get; set; }
		public string PostLookupFilename { get; set; }
		public string ProjectName { get; set; }

		public string Region { get; set; }
		public string CustomProviderName { get; set; }
		public bool UseCustomProviderName { get; set; }
		public bool UsePrivateEndpoint { get; set; }
		public string PrivateEndpoint { get; set; }
		public List<UrlMetadata> Parameters { get; set; }

		public Uri Uri => new TranslationProviderUriBuilder(Constants.MicrosoftProviderScheme).Uri;

		public List<string> LanguagesSupported { get; set; }

		public List<PairMapping> LanguageMappings { get; set; }

		public MicrosoftCredentials MicrosoftCredentials { get; set; }
	}
}
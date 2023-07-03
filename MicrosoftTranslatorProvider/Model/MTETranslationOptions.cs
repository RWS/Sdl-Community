using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using MicrosoftTranslatorProvider.Interfaces;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using static System.Convert;

namespace MicrosoftTranslatorProvider.Model
{
	public class MTETranslationOptions : ITranslationOptions
	{
		public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;
		public bool UseCategoryID { get; set; }
		public bool PersistMicrosoftCredentials { get; set; }
		public bool SendPlainTextOnly { get; set; }
		public bool ResendDrafts { get; set; }
		public bool UsePreEdit { get; set; }
		public bool UsePostEdit { get; set; }
		public string CategoryID { get; set; }
		public string PreLookupFilename { get; set; }
		public string PostLookupFilename { get; set; }
		public string ProjectName { get; set; }
		public string ClientID { get; set; }
		public string Region { get; set; }
		public string CustomProviderName { get; set; }
		public bool UseCustomProviderName { get; set; }
		public bool UsePrivateEndpoint { get; set; }
		public string PrivateEndpoint { get; set; }
		public List<UrlMetadata> Parameters { get; set; }

		public Uri Uri => new TranslationProviderUriBuilder(Constants.MicrosoftProviderScheme).Uri;

		public Dictionary<string, string> LanguagesSupported { get; set; }

		public List<LanguageMapping> LanguageMappings { get; set; }
	}
}
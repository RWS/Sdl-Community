using System;
using System.Collections.Generic;
using GoogleCloudTranslationProvider.Interfaces;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace GoogleCloudTranslationProvider.Models
{
	public class TranslationOptions : ITranslationOptions
	{
		private readonly TranslationProviderUriBuilder _uriBuilder;
		private string _apiKey;

		public TranslationOptions(Uri uri = null)
		{
			_uriBuilder = uri is null ? new TranslationProviderUriBuilder(Constants.GoogleTranslationScheme)
									  : new TranslationProviderUriBuilder(uri);
		}

		#region V2
		[JsonIgnore]
		public string ApiKey
		{
			get => _apiKey;
			set => _apiKey = value;
		}

		public bool PersistGoogleKey { get; set; }

		public List<V2LanguageModel> V2SupportedLanguages { get; set; }
		#endregion

		#region V3
		public string ProjectId { get; set; }

		public string JsonFilePath { get; set; }

		public string ProjectLocation { get; set; }

		public List<LanguagePairResources> LanguageMappingPairs { get; set; }

		public List<V3LanguageModel> V3SupportedLanguages { get; set; }
		#endregion

		#region Common
		public List<string> LanguagesSupported { get; set; }

		public ApiVersion SelectedGoogleVersion { get; set; }

		public Uri Uri => _uriBuilder.Uri;
		#endregion

		#region Settings
		public bool UseCustomProviderName { get; set; }

		public string CustomProviderName { get; set; }

		public bool ResendDrafts { get; set; }

		public bool SendPlainTextOnly { get; set; }

		public bool UsePreEdit { get; set; }

		public string PreLookupFilename { get; set; }

		public bool UsePostEdit { get; set; }

		public string PostLookupFilename { get; set; }

		public string DownloadPath { get; set; }
		#endregion
	}
}
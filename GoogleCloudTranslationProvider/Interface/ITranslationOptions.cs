using System;
using System.Collections.Generic;
using GoogleCloudTranslationProvider.Models;

namespace GoogleCloudTranslationProvider.Interfaces
{
	public interface ITranslationOptions
	{
		bool SendPlainTextOnly { get; set; }

		bool PersistGoogleKey { get; set; }

		bool ResendDrafts { get; set; }

		bool UsePostEdit { get; set; }

		bool UsePreEdit { get; set; }

		bool UseCustomProviderName { get; set; }

		string CustomProviderName { get; set; }

		string PostLookupFilename { get; set; }

		string PreLookupFilename { get; set; }

		string ProjectLocation { get; set; }

		string JsonFilePath { get; set; }

		string DownloadPath { get; set; }

		string ProjectId { get; set; }

		string ApiKey { get; set; }

		List<string> LanguagesSupported { get; set; }

		Uri Uri { get; }

		ApiVersion SelectedGoogleVersion { get; set; }

		List<LanguagePairResources> LanguageMappingPairs { get; set; }
	}
}
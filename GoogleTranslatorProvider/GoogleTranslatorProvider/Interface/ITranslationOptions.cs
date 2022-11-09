using GoogleTranslatorProvider.Models;
using System;
using System.Collections.Generic;

namespace GoogleTranslatorProvider.Interfaces
{
	public interface ITranslationOptions
	{
		bool PersistGoogleKey { get; set; }

		bool SendPlainTextOnly { get; set; }

		bool ResendDrafts { get; set; }

		bool UsePreEdit { get; set; }

		bool UsePostEdit { get; set; }

		bool BasicCsv { get; set; }

		string PreLookupFilename { get; set; }

		string PostLookupFilename { get; set; }

		string JsonFilePath { get; set; }

		string ProjectName { get; set; }

		string GlossaryPath { get; set; }

		string ApiKey { get; set; }

		string GoogleEngineModel { get; set; }

		string ProjectLocation { get; set; }

		ApiVersion SelectedGoogleVersion { get; set; }

		ProviderType SelectedProvider { get; set; }

		Uri Uri { get; }

		Dictionary<string, string> LanguagesSupported { get; set; }
	}
}
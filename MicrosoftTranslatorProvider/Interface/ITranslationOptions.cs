using System;
using System.Collections.Generic;

namespace MicrosoftTranslatorProvider.Interfaces
{
	public interface ITranslationOptions
	{
		bool UseCategoryID { get; set; }

		bool PersistMicrosoftCredentials { get; set; }

		bool SendPlainTextOnly { get; set; }

		bool ResendDrafts { get; set; }

		bool UsePreEdit { get; set; }

		bool UsePostEdit { get; set; }

		bool BasicCsv { get; set; }

		string CategoryID { get; set; }

		string PreLookupFilename { get; set; }

		string PostLookupFilename { get; set; }

		string JsonFilePath { get; set; }

		string ProjectName { get; set; }

		string GlossaryPath { get; set; }

		string ClientID { get; set; }

		string Region { get; set; }

		string ProjectLocation { get; set; }

		string CustomProviderName { get; set; }

		bool UseCustomProviderName { get; set; }

		string PrivateEndpoint { get; set; }

		bool PersistPrivateEndpoint { get; set; }

		Uri Uri { get; }

		Dictionary<string, string> LanguagesSupported { get; set; }
	}
}
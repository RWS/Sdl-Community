using System;
using System.Collections.Generic;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.Interfaces
{
	public interface ITranslationOptions
	{
		bool PersistMicrosoftCredentials { get; set; }

		bool SendPlainTextOnly { get; set; }

		bool ResendDrafts { get; set; }

		bool UsePreEdit { get; set; }

		bool UsePostEdit { get; set; }

		string CategoryID { get; set; }

		string PreLookupFilename { get; set; }

		string PostLookupFilename { get; set; }

		string ProjectName { get; set; }

		string ApiKey { get; set; }

		string Region { get; set; }

		string CustomProviderName { get; set; }

		bool UseCustomProviderName { get; set; }

		bool UsePrivateEndpoint { get; set; }

		string PrivateEndpoint { get; set; }

		List<UrlMetadata> Parameters { get; set; }

		Uri Uri { get; }

		List<string> LanguagesSupported { get; set; }

		List<PairMapping> LanguageMappings { get; set; }

		MicrosoftCredentials MicrosoftCredentials { get; set; }
	}
}
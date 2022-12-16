﻿using System;
using System.Collections.Generic;

namespace GoogleTranslatorProvider.Interfaces
{
	public interface ITranslationOptions
	{
		bool SendPlainTextOnly { get; set; }

		bool PersistGoogleKey { get; set; }

		bool ResendDrafts { get; set; }

		bool UsePostEdit { get; set; }

		bool UsePreEdit { get; set; }

		bool BasicCsv { get; set; }

		string PostLookupFilename { get; set; }

		string PreLookupFilename { get; set; }

		string GoogleEngineModel { get; set; }

		string ProjectLocation { get; set; }

		string JsonFilePath { get; set; }

		string GlossaryPath { get; set; }

		string ProjectId { get; set; }

		string ApiKey { get; set; }

		Dictionary<string, string> LanguagesSupported { get; set; }

		Uri Uri { get; }

		ApiVersion SelectedGoogleVersion { get; set; }

		ProviderType SelectedProvider { get; set; }
	}
}
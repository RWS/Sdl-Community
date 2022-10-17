using System;
using System.Collections.Generic;

namespace MTEnhancedMicrosoftProvider.Interfaces
{
	public interface ITranslationOptions
	{
		bool UseCatID { get; set; }
		bool PersistMicrosoftCreds { get; set; }
		bool SendPlainTextOnly { get; set; }
		bool ResendDrafts { get; set; }
		bool UsePreEdit { get; set; }
		bool UsePostEdit { get; set; }
		bool BasicCsv { get; set; }
		string CatId { get; set; }
		string PreLookupFilename { get; set; }
		string PostLookupFilename { get; set; }
		string JsonFilePath { get; set; }
		string ProjectName { get; set; }
		string GlossaryPath { get; set; }
		string ClientId { get; set; } // Microsoft key
		string Region { get; set; } // Microsoft Region
		string ProjectLocation { get; set; }
		Uri Uri { get; }
		Dictionary<string, string> LanguagesSupported { get; set; }
	}
}
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
		string ClientID { get; set; } // Microsoft key
		string Region { get; set; } // Microsoft Region
		string ProjectLocation { get; set; }
		Uri Uri { get; }
		Dictionary<string, string> LanguagesSupported { get; set; }
	}
}
using System;
using System.Collections.Generic;
using Sdl.Community.MtEnhancedProvider.Helpers;

namespace Sdl.Community.MtEnhancedProvider.Model.Interface
{
	public interface IMtTranslationOptions
	{
		MtTranslationOptions.ProviderType SelectedProvider { get; set; }
		bool UseCatID { get; set; }
		bool PersistGoogleKey { get; set; }
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
		string PeUrl { get; set; } // Microsoft private endpont url
		string ApiKey { get; set; } //Google Key
		string ClientId { get; set; } // Microsoft key
		string Region { get; set; } // Microsoft Region
		string GoogleEngineModel { get; set; }
		string ProjectLocation { get; set; }
		Enums.GoogleApiVersion SelectedGoogleVersion { get; set; }
		Uri Uri { get; }
		Dictionary<string, string> LanguagesSupported { get; set; }
	}
}

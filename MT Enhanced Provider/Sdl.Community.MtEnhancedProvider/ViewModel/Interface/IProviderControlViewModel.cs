using System.Collections.Generic;
using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Model;

namespace Sdl.Community.MtEnhancedProvider.ViewModel.Interface
{
	public delegate void ClearMessageEventRaiser();

	public interface IProviderControlViewModel
	{
		ModelBase ViewModel { get; set; }
		
		ICommand ShowSettingsCommand { get; set; }
		
		List<TranslationOption> TranslationOptions { get; set; }
		
		List<GoogleApiVersion> GoogleApiVersions { get; set; }
		
		GoogleApiVersion SelectedGoogleApiVersion { get; set; }
		
		TranslationOption SelectedTranslationOption { get; set; }
		
		bool IsMicrosoftSelected { get; set; }
		
		bool IsV2Checked { get; set; }
		
		bool UseCatId { get; set; }
		
		bool PersistGoogleKey { get; set; }
		
		bool PersistMicrosoftKey { get; set; }
		
		bool IsTellMeAction { get; set; }
		
		bool BasicCsvGlossary { get; set; }
		
		string CatId { get; set; }
		
		string ApiKey { get; set; } //Google
	
		string PeUrl { get; set;  } // Microsoft private endpoint url

		string ClientId { get; set; } //Microsoft

		SubscriptionRegion Region { get; set; }

		string JsonFilePath { get; set; }
		
		string ProjectName { get; set; }
		
		string GoogleEngineModel { get; set; }
		
		string ProjectLocation { get; set; }
		
		string GlossaryId { get; set; }
		
		string GlossaryPath { get; set; }
		
		event ClearMessageEventRaiser ClearMessageRaised;
	}
}

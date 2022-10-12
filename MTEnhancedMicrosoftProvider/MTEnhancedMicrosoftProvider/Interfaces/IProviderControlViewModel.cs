using System.Collections.Generic;
using System.Windows.Input;
using MTEnhancedMicrosoftProvider.Model;

namespace MTEnhancedMicrosoftProvider.Interfaces
{
	public delegate void ClearMessageEventRaiser();

	public interface IProviderControlViewModel
	{
		BaseModel ViewModel { get; set; }
		
		ICommand ShowSettingsCommand { get; set; }
		
		List<TranslationOption> TranslationOptions { get; set; }
		
		TranslationOption SelectedTranslationOption { get; set; }
		
		bool IsMicrosoftSelected { get; set; }
		
		bool UseCatId { get; set; }
		
		bool PersistGoogleKey { get; set; }
		
		bool PersistMicrosoftKey { get; set; }
		
		bool IsTellMeAction { get; set; }
		
		bool BasicCsvGlossary { get; set; }
		
		string CatId { get; set; }
		
		string ClientId { get; set; } //Microsoft

		RegionSubscription Region { get; set; }

		string JsonFilePath { get; set; }
		
		string ProjectName { get; set; }
		
		string ProjectLocation { get; set; }
		
		string GlossaryId { get; set; }
		
		string GlossaryPath { get; set; }
		
		event ClearMessageEventRaiser ClearMessageRaised;
	}
}

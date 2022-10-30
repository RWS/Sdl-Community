using System.Collections.Generic;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.Interfaces
{
	public delegate void ClearMessageEventRaiser();

	public interface IProviderControlViewModel
	{
		bool IsMicrosoftSelected { get; set; }
		
		bool UseCategoryID { get; set; }
		
		bool PersistMicrosoftKey { get; set; }
		
		bool IsTellMeAction { get; set; }
		
		bool BasicCsvGlossary { get; set; }
		
		string CategoryID { get; set; }
		
		string ClientID { get; set; }

		string JsonFilePath { get; set; }
		
		string ProjectName { get; set; }
		
		string ProjectLocation { get; set; }
		
		string GlossaryId { get; set; }
		
		string GlossaryPath { get; set; }

		BaseModel ViewModel { get; set; }
		
		RegionSubscription Region { get; set; }
		
		List<TranslationOption> TranslationOptions { get; set; }
		
		TranslationOption SelectedTranslationOption { get; set; }
		
		ICommand ShowSettingsCommand { get; set; }
		
		event ClearMessageEventRaiser ClearMessageRaised;
	}
}
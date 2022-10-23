using System.Collections.Generic;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.Interfaces
{
	public delegate void ClearMessageEventRaiser();

	public interface IProviderControlViewModel
	{
		BaseModel ViewModel { get; set; }
		
		ICommand ShowSettingsCommand { get; set; }
		
		List<TranslationOption> TranslationOptions { get; set; }
		
		TranslationOption SelectedTranslationOption { get; set; }
		
		bool IsMicrosoftSelected { get; set; }
		
		bool UseCategoryID { get; set; }
		
		bool PersistMicrosoftKey { get; set; }
		
		bool IsTellMeAction { get; set; }
		
		bool BasicCsvGlossary { get; set; }
		
		string CategoryID { get; set; }
		
		string ClientID { get; set; }

		RegionSubscription Region { get; set; }

		string JsonFilePath { get; set; }
		
		string ProjectName { get; set; }
		
		string ProjectLocation { get; set; }
		
		string GlossaryId { get; set; }
		
		string GlossaryPath { get; set; }
		
		event ClearMessageEventRaiser ClearMessageRaised;
	}
}

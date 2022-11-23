using GoogleTranslatorProvider.Models;
using System.Windows.Input;

namespace GoogleTranslatorProvider.Interfaces
{
	public interface ISettingsControlViewModel
	{
		BaseModel ViewModel { get; set; }

		bool ReSendDraft { get; set; }
		
		bool DoPreLookup { get; set; }
		
		bool DoPostLookup { get; set; }
		
		bool SendPlainText { get; set; }
		
		bool IsTellMeAction { get; set; }
		
		string ErrorMessage { get; set; }
		
		string PreLookupFileName { get; set; }
		
		string PostLookupFileName { get; set; }
		
		ICommand ShowMainWindowCommand { get; set; }
		
		ICommand ShowSettingsCommand { get; set; }
		
		ICommand BrowseCommand { get; }
	}
}
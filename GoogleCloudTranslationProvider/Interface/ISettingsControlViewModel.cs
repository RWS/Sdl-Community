using System.Windows.Input;
using GoogleCloudTranslationProvider.ViewModel;

namespace GoogleCloudTranslationProvider.Interfaces
{
	public interface ISettingsControlViewModel
	{
		BaseViewModel ViewModel { get; set; }

		bool ReSendDraft { get; set; }
		
		bool DoPreLookup { get; set; }
		
		bool DoPostLookup { get; set; }
		
		bool SendPlainText { get; set; }
		
		bool IsTellMeAction { get; set; }

		bool UseCustomProviderName { get; set; }
		
		string PreLookupFileName { get; set; }
		
		string PostLookupFileName { get; set; }

		string CustomProviderName { get; set; }
		
		ICommand BrowseFileCommand { get; }

		ICommand ClearCommand { get; }

		bool SettingsAreValid();
	}
}
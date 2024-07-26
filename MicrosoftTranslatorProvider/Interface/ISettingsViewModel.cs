using System.Windows.Input;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.Interfaces
{
	public interface ISettingsViewModel
	{
		bool ReSendDraft { get; set; }

		bool SendPlainText { get; set; }

		bool DoPreLookup { get; set; }

		bool DoPostLookup { get; set; }

		string PreLookupFileName { get; set; }

		string PostLookupFileName { get; set; }

		string ErrorMessage { get; set; }

		string CustomProviderName { get; set; }

		bool UseCustomProviderName { get; set; }

		BaseModel ViewModel { get; }

		ICommand ShowMainWindowCommand { get; set; }

		ICommand BrowseCommand { get; set; }

		ICommand ShowSettingsCommand { get; set; }
	}
}
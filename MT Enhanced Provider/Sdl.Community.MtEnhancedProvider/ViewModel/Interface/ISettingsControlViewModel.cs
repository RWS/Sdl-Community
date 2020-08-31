using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Model;

namespace Sdl.Community.MtEnhancedProvider.ViewModel.Interface
{
	public interface ISettingsControlViewModel
	{
		ModelBase ViewModel { get; set; }
		ICommand ShowMainWindowCommand { get; set; }
		ICommand BrowseCommand { get; set; }
		ICommand ShowSettingsCommand { get; set; }
		bool ReSendDraft { get; set; }
		bool SendPlainText { get; set; }
		bool DoPreLookup { get; set; }
		bool DoPostLookup { get; set; }
		string PreLookupFileName { get; set; }
		string PostLookupFileName { get; set; }
		string ErrorMessage { get; set; }
	}
}

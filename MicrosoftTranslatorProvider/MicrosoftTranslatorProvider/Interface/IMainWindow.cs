using System.Collections.Generic;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.Interfaces
{
	public interface IMainWindow
	{
		bool DialogResult { get; set; }
		
		string ErrorMessage { get; set; }
		
		string TranslatorErrorResponse { get; set; }

		ViewDetails SelectedView { get; set; }

		List<ViewDetails> AvailableViews { get; set; }
		
		ITranslationOptions Options { get; set; }
		
		ICommand ShowSettingsViewCommand { get; set; }
		
		ICommand ShowMainViewCommand { get; set; }
		
		ICommand SaveCommand { get; set; }
		
		void AddEncriptionMetaToResponse(string response);

		bool IsWindowValid();
	}
}
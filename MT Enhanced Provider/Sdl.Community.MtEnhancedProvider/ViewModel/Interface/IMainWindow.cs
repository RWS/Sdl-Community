using System.Collections.Generic;
using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;

namespace Sdl.Community.MtEnhancedProvider.ViewModel.Interface
{
	public interface IMainWindow
	{
		ViewDetails SelectedView { get; set; }
		List<ViewDetails> AvailableViews { get; set; }
		ICommand ShowSettingsViewCommand { get; set; }
		ICommand ShowMainViewCommand { get; set; }
		ICommand SaveCommand { get; set; }
		IMtTranslationOptions Options { get; set; }
		bool DialogResult { get; set; }
		string ErrorMessage { get; set; }
		void AddEncriptionMetaToResponse(string response);
		bool IsWindowValid();
	}
}

using System.Collections.Generic;
using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Model;

namespace Sdl.Community.MtEnhancedProvider.ViewModel.Interface
{
	public interface IMainWindow
	{
		ViewDetails SelectedView { get; set; }
		List<ViewDetails> AvailableViews { get; set; }
		ICommand ShowSettingsViewCommand { get; set; }
		ICommand ShowMainViewCommand { get; set; }
		ICommand SaveCommand { get; set; }
	}
}

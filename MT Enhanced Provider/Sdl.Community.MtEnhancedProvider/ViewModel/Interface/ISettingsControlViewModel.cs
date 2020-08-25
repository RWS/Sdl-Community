using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Model;

namespace Sdl.Community.MtEnhancedProvider.ViewModel.Interface
{
	public interface ISettingsControlViewModel
	{
		ModelBase ViewModel { get; set; }
		ICommand ShowMainWindowCommand { get; set; }
	}
}

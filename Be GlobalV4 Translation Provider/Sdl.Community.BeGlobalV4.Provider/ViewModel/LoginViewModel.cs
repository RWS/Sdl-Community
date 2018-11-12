using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Ui;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class LoginViewModel : BaseViewModel
	{
		private readonly BeGlobalWindow _mainWindow;


		public LoginViewModel(BeGlobalWindow mainWindow)
		{
			_mainWindow = mainWindow;
		}  
	}
}

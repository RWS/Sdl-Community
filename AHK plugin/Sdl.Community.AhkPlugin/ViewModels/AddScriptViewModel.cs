using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.AhkPlugin.Ui;

namespace Sdl.Community.AhkPlugin.ViewModels
{
    public class AddScriptViewModel: ViewModelBase
    {
	    private readonly MainWindowViewModel _mainWindowViewModel;
	    private ICommand _backCommand;
		public AddScriptViewModel(MainWindowViewModel mainWindowViewModel)
		{
			_mainWindowViewModel = mainWindowViewModel;
		}

	    public ICommand BackCommand => _backCommand ?? (_backCommand = new CommandHandler(BackToScriptsList, true));

	    private void BackToScriptsList()
	    {
		    _mainWindowViewModel.LoadScriptsPage();
	    }
	}
}

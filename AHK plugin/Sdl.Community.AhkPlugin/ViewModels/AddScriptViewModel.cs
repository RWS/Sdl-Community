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
	    private ICommand _insertCommand;
		public AddScriptViewModel(MainWindowViewModel mainWindowViewModel)
		{
			_mainWindowViewModel = mainWindowViewModel;
		}

	    public ICommand BackCommand => _backCommand ?? (_backCommand = new CommandHandler(BackToScriptsList, true));
	    public ICommand InsertCommand => _insertCommand ?? (_insertCommand = new CommandHandler(InsertScript, true));

	    private void InsertScript()
	    {
		    
	    }

	    private void BackToScriptsList()
	    {
		    _mainWindowViewModel.LoadScriptsPage();
	    }
	}
}

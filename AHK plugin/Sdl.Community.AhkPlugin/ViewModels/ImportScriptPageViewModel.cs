using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AhkPlugin.ViewModels
{
    public class ImportScriptPageViewModel:ViewModelBase
    {
	    private readonly MainWindowViewModel _mainWindowViewModel;


		public ImportScriptPageViewModel(MainWindowViewModel mainWindowViewModel)
		{
			_mainWindowViewModel = mainWindowViewModel;
		}
    }
}

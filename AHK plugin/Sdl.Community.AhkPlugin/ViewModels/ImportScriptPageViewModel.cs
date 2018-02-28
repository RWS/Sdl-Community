using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.AhkPlugin.Helpers;

namespace Sdl.Community.AhkPlugin.ViewModels
{
    public class ImportScriptPageViewModel:ViewModelBase
    {
	    private readonly MainWindowViewModel _mainWindowViewModel;
	    private ICommand _backCommand;
	    private ICommand _dragEnterCommand;

		public ImportScriptPageViewModel(MainWindowViewModel mainWindowViewModel)
		{
			_mainWindowViewModel = mainWindowViewModel;
		}
	    public ICommand BackCommand => _backCommand ?? (_backCommand = new CommandHandler(BackToScriptsList, true));

		public ICommand DragEnterCommand => _dragEnterCommand ??
											(_dragEnterCommand = new RelayCommand(HandlePreviewDrop));

		private void HandlePreviewDrop(object inObject)
	    {
			var ido = inObject as IDataObject;
		    if (null == ido) return;

		    var text = ido.GetData(DataFormats.FileDrop);
		    string[] docPath = (string[])ido.GetData(DataFormats.FileDrop);
		    var defaultFormat = DataFormats.Text;
		    if (File.Exists(docPath[0]))
		    {
			    try
			    {
				    var test = File.ReadAllText(docPath[0]);
			    }
			    catch (System.Exception)
			    {
				  
			    }
		    }
		}
		private void BackToScriptsList()
	    {
		    _mainWindowViewModel.LoadScriptsPage();
	    }
	}
}

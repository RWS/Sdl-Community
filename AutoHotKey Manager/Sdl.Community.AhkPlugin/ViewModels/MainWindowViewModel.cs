using Sdl.Community.AhkPlugin.Interface;
using Sdl.Community.AhkPlugin.Model;
using Sdl.Community.AhkPlugin.Service;

namespace Sdl.Community.AhkPlugin.ViewModels
{
   public  class MainWindowViewModel : ViewModelBase
	{ 
		private ViewModelBase _currentViewModel;
		private IDialogService _dialogService;

		public ViewModelBase CurrentViewModel
		{
			get => _currentViewModel;
			set
			{
				_currentViewModel = value;
				OnPropertyChanged(nameof(CurrentViewModel));
			}
		}
		public MainWindowViewModel()
		{
			_dialogService = new FilesDialogService();
			LoadScriptsPage();
		}

		public void LoadAddScriptPage()
		{
			CurrentViewModel = new AddScriptViewModel(this);
		}
		public void LoadScriptsPage()
		{
			CurrentViewModel = new ScriptsWindowViewModel(this);
		}

		public void LoadImportPage()
		{
			CurrentViewModel = new ImportScriptPageViewModel(this, _dialogService);
		}

		public void LoadEditPage(Script script)
		{
			CurrentViewModel = new EditScriptPageViewModel(script,this);
		}
	}
}

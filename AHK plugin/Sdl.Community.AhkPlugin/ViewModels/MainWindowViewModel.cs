using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.AhkPlugin.Annotations;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.AhkPlugin.Model;
using Sdl.Community.AhkPlugin.Ui;

namespace Sdl.Community.AhkPlugin.ViewModels
{
   public  class MainWindowViewModel : ViewModelBase
	{ 
		private ViewModelBase _currentViewModel;

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
			CurrentViewModel = new ImportScriptPageViewModel(this);
		}

		public void LoadEditPage(Script script)
		{
			CurrentViewModel = new EditScriptPageViewModel(script,this);
		}
	}
}

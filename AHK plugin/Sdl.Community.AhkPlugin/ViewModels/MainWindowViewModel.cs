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
using Sdl.Community.AhkPlugin.Ui;

namespace Sdl.Community.AhkPlugin.ViewModels
{
   public  class MainWindowViewModel : ViewModelBase,INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private ScriptsWindowViewModel _scriptsListViewModel;
		public ICommand LoadAddPageCommand { get; }
		public ScriptsWindowViewModel ScriptsWindowViewModel { get; set; }
		public AddScriptViewModel AddScriptViewModel { get; set; }

		public ICommand LoadScriptsPageCommand { get; }

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
			LoadAddPageCommand = new CommandHandler(LoadAddScriptPage,true);
			LoadScriptsPageCommand = new CommandHandler(LoadScriptsPage,true);
		}

		public void LoadAddScriptPage()
		{
			CurrentViewModel = new AddScriptViewModel(this);
		}
		public void LoadScriptsPage()
		{
			CurrentViewModel = new ScriptsWindowViewModel(this);
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

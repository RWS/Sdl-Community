using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using Sdl.Community.Reports.Viewer.Commands;
using Sdl.Community.Reports.Viewer.Model;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class SettingsViewModel : BaseModel, IDisposable
	{		
		private Window _window;
		private string _windowTitle;
		private ICommand _saveCommand;
		private ICommand _resetCommand;

		

		public SettingsViewModel(Window window)
		{
			_window = window;
			WindowTitle = "Settings";
		
		}
		
		public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new CommandHandler(SaveChanges));

		public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new CommandHandler(Reset));

	
		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

	
		private void SaveChanges(object parameter)
		{
	
		}

		private void Reset(object paramter)
		{
		
		}


		



		public void Dispose()
		{
			
		}
	}
}

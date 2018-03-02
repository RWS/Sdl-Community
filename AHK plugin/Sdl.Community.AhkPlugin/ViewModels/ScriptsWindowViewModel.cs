using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.AhkPlugin.Annotations;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.AhkPlugin.ItemTemplates;
using Sdl.Community.AhkPlugin.Model;
using Sdl.Community.AhkPlugin.Repository.DataBase;
using Sdl.Community.AhkPlugin.Ui;

namespace Sdl.Community.AhkPlugin.ViewModels
{
	public class ScriptsWindowViewModel: ViewModelBase
	{
		private static  MainWindowViewModel _mainWindow;
		private ObservableCollection<Script> _scriptsCollection = new ObservableCollection<Script>();
		private ICommand _addCommand;
		private ICommand _importCommand;
		private ICommand _changeScriptStateCommand;
		private ICommand _removeScriptCommand;
		private readonly ScriptDb _scriptsDb;

		public ScriptsWindowViewModel(MainWindowViewModel mainWindowViewModel)
		{
			_mainWindow = mainWindowViewModel;
		}

		public ScriptsWindowViewModel()
		{
			 _scriptsDb = new ScriptDb();
			var savedScripts = _scriptsDb.GetAllScripts().Result;

			foreach (var script in savedScripts)
			{
				ScriptsCollection.Add(script);
			}
		}

		public ICommand AddCommand => _addCommand ?? (_addCommand = new CommandHandler(AddScriptAction, true));
		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(ImportAction, true));
		public ICommand ChangeScriptStateCommand => _changeScriptStateCommand ?? (_changeScriptStateCommand = new RelayCommand(ChangeState));

		public ICommand RemoveScriptCommand => _removeScriptCommand ??
		                                       (_removeScriptCommand = new CommandHandler(RemoveScripts, true));

		private void RemoveScripts()
		{
			var messageResult = MessageBox.Show("Are you sure you want to delete selected scripts?", "Confirmation",
				MessageBoxButton.OKCancel, MessageBoxImage.Question);
			if (messageResult != MessageBoxResult.OK) return;
			var scriptsToBeRemoved = ScriptsCollection.Where(s => s.IsSelected).ToList();
			//remove from UI
			foreach (var script in scriptsToBeRemoved)
			{
				ScriptsCollection.Remove(script);
			}
			//Remove from db
			_scriptsDb.RemoveScripts(scriptsToBeRemoved);
		}

		private void AddScriptAction()
		{
			_mainWindow.LoadAddScriptPage();
		}

		private void ChangeState(object row)
		{
			
		}

		private void ImportAction()
		{
			_mainWindow.LoadImportPage();
		}

		public ObservableCollection<Script> ScriptsCollection
		{
			get => _scriptsCollection;

			set
			{
				if (Equals(value, _scriptsCollection))
				{
					return;
				}
				_scriptsCollection = value;
				OnPropertyChanged(nameof(ScriptsCollection));
			}
		}
	}
}

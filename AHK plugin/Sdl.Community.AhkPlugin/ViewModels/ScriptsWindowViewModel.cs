using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.AhkPlugin.Annotations;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.AhkPlugin.ItemTemplates;
using Sdl.Community.AhkPlugin.Model;
using Sdl.Community.AhkPlugin.Repository.DataBase;
using Sdl.Community.AhkPlugin.Ui;
using MessageBox = System.Windows.MessageBox;

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
		private ICommand _exportCommand;
		private ICommand _changeScriptPathCommand;
		private ICommand _selectAllCommand;
		private ICommand _editScriptCommand;
		private bool _selectAll;
		private readonly ScriptDb _scriptsDb;
		private readonly MasterScriptDb _masterScriptDb;

		public static readonly Log Log = Log.Instance;
		
		public ScriptsWindowViewModel(MainWindowViewModel mainWindowViewModel)
		{
			_mainWindow = mainWindowViewModel;
		}

		public ScriptsWindowViewModel()
		{
			_scriptsDb = new ScriptDb();
			_masterScriptDb = new MasterScriptDb();
			try
			{
				var savedScripts = _masterScriptDb.GetMasterScript().Result.Scripts;
				ScriptsCollection = new ObservableCollection<Script>(savedScripts);
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.GetMasterScript}: {Constants.Message}: {ex.Message}\n " +
					$"{Constants.StackTrace}: {ex.StackTrace}\n {Constants.InnerException}: {ex.InnerException}");
			}
		}

		public ICommand AddCommand => _addCommand ?? (_addCommand = new CommandHandler(AddScriptAction, true));
		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(ImportAction, true));
		public ICommand ChangeScriptStateCommand => _changeScriptStateCommand ?? (_changeScriptStateCommand = new RelayCommand(ChangeState));
		public ICommand EditScriptCommand => _editScriptCommand ?? (_editScriptCommand = new RelayCommand(EditScript));
		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(ExportScripts, true));

		public ICommand ChangeScriptPath => _changeScriptPathCommand ??
		                                    (_changeScriptPathCommand = new CommandHandler(ChangePath, true));

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllScripts, true));


		private void SelectAllScripts()
		{
			Helpers.Ui.Select(ScriptsCollection,SelectAll);
		}

		private async void ChangePath()
		{
			try
			{
				var folderDialog = new FolderSelectDialog
				{
					Title = "Select a folder where the master script should be saved"
				};
				var folderPath = string.Empty;
				if (folderDialog.ShowDialog())
				{
					folderPath = folderDialog.FileName;
				}
				if (!string.IsNullOrEmpty(folderPath))
				{
					var masterScript = await _masterScriptDb.GetMasterScript();
					masterScript.Location = folderPath;
					await _masterScriptDb.UpdateScript(masterScript);

					ProcessScript.ExportScript(Path.Combine(masterScript.Location, masterScript.Name), masterScript.Scripts);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.ChangePath}: {Constants.Message}: {ex.Message}\n " +
					$"{Constants.StackTrace}: {ex.StackTrace}\n {Constants.InnerException}: {ex.InnerException}");
			}
		}

		private void ExportScripts()
		{
			try
			{
				if (ScriptsCollection.Any(s => s.IsSelected))
				{
					var folderDialog = new SaveFileDialog
					{
						Title = @"Select a location where the script should be exported",
						DefaultExt = "ahk"
					};
					if (folderDialog.ShowDialog() == DialogResult.OK)
					{
						var folderPath = folderDialog.FileName;
						var selectedScripts = ScriptsCollection.Where(s => s.IsSelected).ToList();
						ProcessScript.ExportScript(Path.Combine(folderPath, folderDialog.FileName), selectedScripts);
						MessageBox.Show("Script was exported successfully to selected location", "",
							MessageBoxButton.OK, MessageBoxImage.Information);
						Helpers.Ui.Select(ScriptsCollection, false);
					}
				}
				else
				{
					MessageBox.Show("Please select at least one script from the grid to export", "Warning",
						MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.ExportScripts}: {Constants.Message}: {ex.Message}\n " +
					$"{Constants.StackTrace}: {ex.StackTrace}\n {Constants.InnerException}: {ex.InnerException}");
			}
		}

		public ICommand RemoveScriptCommand => _removeScriptCommand ??
		                                       (_removeScriptCommand = new CommandHandler(RemoveScripts, true));

		private void RemoveScripts()
		{
			try
			{
				if (ScriptsCollection.Any(s => s.IsSelected))
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
					_masterScriptDb.RemoveScripts(scriptsToBeRemoved);
					//write masterscript on the disk
					var masterScript = _masterScriptDb.GetMasterScript().Result;
					ProcessScript.ExportScript(Path.Combine(masterScript.Location, masterScript.Name), masterScript.Scripts);
				}
				else
				{
					MessageBox.Show("Please select at least one script from the grid to be removed", "Warning",
					   MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.RemoveScripts}: {Constants.Message}: {ex.Message}\n " +
					$"{Constants.StackTrace}: {ex.StackTrace}\n {Constants.InnerException}: {ex.InnerException}");
			}
		}

		private void AddScriptAction()
		{
			_mainWindow.LoadAddScriptPage();
		}

		private void ChangeState(object row)
		{
			try
			{
				var script = (Script)row;
				if (script != null)
				{
					Helpers.Ui.SetStateColors(script);
					ProcessScript.ChangeScriptState(script);
					ProcessScript.SaveScriptToMaster(script);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.ChangeState}: {Constants.Message}: {ex.Message}\n " +
					$"{Constants.StackTrace}: {ex.StackTrace}\n {Constants.InnerException}: {ex.InnerException}");
			}
		}

		private void EditScript(object row)
		{
			var script = (Script)row;
			if (script != null)
			{
				_mainWindow.LoadEditPage(script);
			}
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
		public bool SelectAll
		{
			get => _selectAll;

			set
			{
				if (Equals(value, _selectAll))
				{
					return;
				}
				_selectAll = value;
				OnPropertyChanged(nameof(SelectAll));
			}
		}
	}
}

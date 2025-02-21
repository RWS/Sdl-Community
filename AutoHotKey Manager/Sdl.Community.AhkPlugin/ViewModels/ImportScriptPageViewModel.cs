﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.AhkPlugin.Interface;
using Sdl.Community.AhkPlugin.ItemTemplates;
using Sdl.Community.AhkPlugin.Model;
using Sdl.Community.AhkPlugin.Service;

namespace Sdl.Community.AhkPlugin.ViewModels
{
    public class ImportScriptPageViewModel:ViewModelBase
    {
	    private static  MainWindowViewModel _mainWindowViewModel;
	    private ICommand _backCommand;
	    private ICommand _dragEnterCommand;
	    private ICommand _removeFileCommand;
	    private ICommand _addFilesCommand;
	    private ICommand _addToMasterCommand;
	    private ICommand _changeScriptStateCommand;
	    private ICommand _selectAllCommand;
	    private readonly IDialogService _dialogService;
		private readonly DbContext _dbContext;
		private string _gridVisibility;
	    private string _message;
	    private string _messageVisibility;
	    private bool _selectAll;
		private ObservableCollection<KeyValuePair<string,Script>> _scriptsCollection = new ObservableCollection<KeyValuePair<string, Script>>();
		private ObservableCollection<ImportScriptItemTemplate> _filesNameCollection = new ObservableCollection<ImportScriptItemTemplate>();
		private const string FilesFilter = "AHK Scripts(*.ahk) | *.ahk";

		public static readonly Log Log = Log.Instance;

		public ImportScriptPageViewModel(MainWindowViewModel mainWindowViewModel, IDialogService dialogService)
		{
			_mainWindowViewModel = mainWindowViewModel;
			_dialogService = dialogService;
			_dbContext = new DbContext();
			GridVisibility = "Collapsed";
			MessageVisibility = "Collapsed";
		}

		public ICommand BackCommand => _backCommand ?? (_backCommand = new CommandHandler(BackToScriptsList, true));

		public ICommand DragEnterCommand => _dragEnterCommand ??
											(_dragEnterCommand = new RelayCommand(HandlePreviewDrop));

	    public ICommand RemoveFileCommand => _removeFileCommand ?? (_removeFileCommand = new RelayCommand(RemoveFile));

	    public ICommand AddToMasterCommand => _addToMasterCommand ??
	                                          (_addToMasterCommand = new CommandHandler(ImportScriptsToMaster, true));

	    public ICommand ChangeScriptStateCommand => _changeScriptStateCommand ?? (_changeScriptStateCommand = new RelayCommand(ChangeState));

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllScripts, true));

		public ICommand AddFilesCommand => _addFilesCommand ?? (_addFilesCommand = new CommandHandler(AddFiles, true));

		public ObservableCollection<ImportScriptItemTemplate> FilesNameCollection
		{
			get => _filesNameCollection;

			set
			{
				if (Equals(value, _filesNameCollection))
				{
					return;
				}
				_filesNameCollection = value;
				OnPropertyChanged(nameof(FilesNameCollection));
			}
		}

		public ObservableCollection<KeyValuePair<string, Script>> ScriptsCollection
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

		public string GridVisibility
		{
			get => _gridVisibility;

			set
			{
				if (Equals(value, _gridVisibility))
				{
					return;
				}
				_gridVisibility = value;
				OnPropertyChanged(nameof(GridVisibility));
			}
		}

		public string MessageVisibility
		{
			get => _messageVisibility;

			set
			{
				if (Equals(value, _messageVisibility))
				{
					return;
				}
				_messageVisibility = value;
				OnPropertyChanged(nameof(MessageVisibility));
			}
		}

		public string Message
		{
			get => _message;

			set
			{
				if (Equals(value, _message))
				{
					return;
				}
				_message = value;
				OnPropertyChanged(nameof(Message));
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
				ToggleCheckAllFiles(value);
				_selectAll = value;
				OnPropertyChanged(nameof(SelectAll));
			}
		}

		private void SelectAllScripts()
	    {
		    Helpers.Ui.Select(GetObservableCollectionOfScripts(), SelectAll);
	    }

		private ObservableCollection<Script> GetObservableCollectionOfScripts()
	    {
			var scripts = ScriptsCollection.Select(s => s.Value).ToList();
		    return new ObservableCollection<Script>(scripts);
	    }

		private async void AddFiles()
		{
			var selectedFilesPaths = _dialogService.ShowDialog(FilesFilter);
			if (!(selectedFilesPaths is null))
			{
				await AddScriptsToCollection(selectedFilesPaths);
			}
			
			SetGridVisibility();
		}

		private async void ImportScriptsToMaster()
	    {
			try
			{
				var scriptsToBeImported = ScriptsCollection.Where(s => s.Value.IsSelected).Select(s => s.Value).ToList();
				if (scriptsToBeImported.Count > 0)
				{
					foreach (var script in scriptsToBeImported)
					{
						script.IsSelected = false;
						script.ScriptStateAction = script.Active ? "Disable" : "Enable";
						script.RowColor = script.Active ? "Black" : "DarkGray";

						//await _dbContext.AddNewScript(script);
						RemoveScriptFromGrid(script);
					}

					var filesToRemove = FilesNameCollection.Where(template =>
						ScriptsCollection.All(s =>
							s.Value.FileName != Path.GetFileNameWithoutExtension(template.FilePath))).ToList();

					foreach (var file in filesToRemove)
					{
						FilesNameCollection.Remove(file);
					}

					var masterScript = await _dbContext.GetMasterScript();
					masterScript.Scripts.AddRange(scriptsToBeImported);
					await _dbContext.UpdateScript(masterScript);
					//write masterscript on the disk
					ProcessScript.ExportScript(Path.Combine(masterScript.Location, masterScript.Name), masterScript.Scripts);

					Message = "Scripts imported successfully";
					MessageVisibility = "Visible";
					Helpers.Ui.Select(GetObservableCollectionOfScripts(), false);
					SelectAll= false;
					SetGridVisibility();
				}
				else
				{
					MessageBox.Show("Please select at least one script from the grid to import", "Warning",
					   MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.ImportScript}: {Constants.Message}: {ex.Message}\n " +
					$"{Constants.StackTrace}: {ex.StackTrace}\n {Constants.InnerException}: {ex.InnerException}");
			}
		}

		private void RemoveScriptFromGrid(Script script)
		{
			try
			{
				var scriptToBeRemoved = ScriptsCollection.FirstOrDefault(s => s.Value.ScriptId.Equals(script.ScriptId));
				ScriptsCollection.Remove(scriptToBeRemoved);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.RemoveScriptFromGrid}: {Constants.Message}: {ex.Message}\n " +
					$"{Constants.StackTrace}: {ex.StackTrace}\n {Constants.InnerException}: {ex.InnerException}");
			}
		}

	    private void ChangeState(object row)
	    {
			try
			{
				var script = ((KeyValuePair<string, Script>)row).Value;
				if (script == null) return;
				Helpers.Ui.SetStateColors(script);
				ProcessScript.ChangeScriptState(script);

				var scriptToBeUpdated = ScriptsCollection.FirstOrDefault(s => s.Value.ScriptId.Equals(script.ScriptId)).Value;
				scriptToBeUpdated.Active = script.Active;
				scriptToBeUpdated.Text = script.Text;
				scriptToBeUpdated.RowColor = script.RowColor;
				scriptToBeUpdated.ScriptStateAction = script.ScriptStateAction;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.ChangeState}: {Constants.Message}: {ex.Message}\n " +
					$"{Constants.StackTrace}: {ex.StackTrace}\n {Constants.InnerException}: {ex.InnerException}");
			}
		}

		private void RemoveFile(object file)
	    {
			try
			{
				if (file != null)
				{
					var filePath = (string)file;
					var fileName = Path.GetFileNameWithoutExtension(filePath);
					var fileToRemove = FilesNameCollection.FirstOrDefault(f => f.FilePath.Equals(filePath));
					if (fileToRemove != null)
					{
						FilesNameCollection.Remove(fileToRemove);
						//remove the scripts which coresponds with the removed file from the grid
						var scriptsToBeRemoved = ScriptsCollection.Where(s => s.Key.Equals(fileName)).ToList();
						foreach (var script in scriptsToBeRemoved)
						{
							ScriptsCollection.Remove(script);
						}

					}
					if (ScriptsCollection.Count.Equals(0))
					{
						MessageVisibility = "Collapsed";

					}
				}
				SetGridVisibility();
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.RemoveFile}: {Constants.Message}: {ex.Message}\n " +
					$"{Constants.StackTrace}: {ex.StackTrace}\n {Constants.InnerException}: {ex.InnerException}");
			}
		}

		private async void HandlePreviewDrop(object droppedFile)
	    {
			var file = droppedFile as IDataObject;
		    if (null == file) return;
		    var documentsPath = (string[])file.GetData(DataFormats.FileDrop);

		    if (documentsPath != null)
		    {
			    await AddScriptsToCollection(documentsPath.ToList());
		    }
		    SetGridVisibility();
	    }

		private async Task AddScriptsToCollection(IEnumerable<string> documentsPath)
		{
			foreach (var path in documentsPath)
			{
				if (ProcessScript.IsGeneratedByAhkPlugin(path))
				{
					MessageVisibility = "Collapsed";
					var pathAlreadyAdded = FilesNameCollection.Any(p => p.FilePath.Equals(path));
					if (!pathAlreadyAdded)
					{
						var scripts = ProcessScript.ReadImportedScript(path);
						foreach (var script in scripts)
						{
							var exist = await ProcessScript.ScriptContentAlreadyExist(script.Value);
							if (!exist)
							{
								script.Value.ScriptStateAction = script.Value.Active ? "Disable" : "Enable";
								script.Value.RowColor = script.Value.Active ? "Black" : "DarkGray";
								ScriptsCollection.Add(script);
							}
						}

						var newFile = new ImportScriptItemTemplate
						{
							Content = Path.GetFileNameWithoutExtension(path),
							RemoveFileCommand = new RelayCommand(RemoveFile),
							FilePath = path
						};
						FilesNameCollection.Add(newFile);
						if (ScriptsCollection.Count.Equals(0))
						{
							MessageVisibility = "Visible";
							Message = "Imported scripts are already in the master script.";
						}
					}
				}
				else
				{
					MessageVisibility = "Visible";
					Message = "Only scripts generated by AHK Plugin are supported.";
				}
			}
		}

		private void SetGridVisibility()
	    {
			GridVisibility = ScriptsCollection.Count > 0 ? "Visible" : "Collapsed";
		}
		private void BackToScriptsList()
	    {
		    _mainWindowViewModel.LoadScriptsPage();
	    }

	    private void ToggleCheckAllFiles(bool value)
	    {
		    foreach (var script in ScriptsCollection)
		    {
			    script.Value.IsSelected = value;
		    }
	    }
	}
}

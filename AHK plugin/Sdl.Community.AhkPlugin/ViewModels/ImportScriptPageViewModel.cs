using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Raven.Abstractions.Extensions;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.AhkPlugin.ItemTemplates;
using Sdl.Community.AhkPlugin.Model;
using Sdl.Community.AhkPlugin.Repository.DataBase;


namespace Sdl.Community.AhkPlugin.ViewModels
{
    public class ImportScriptPageViewModel:ViewModelBase
    {
	    private static  MainWindowViewModel _mainWindowViewModel;
	    private ICommand _backCommand;
	    private ICommand _dragEnterCommand;
	    private ICommand _removeFileCommand;
	    private ICommand _addToMasterCommand;
	    private ICommand _changeScriptStateCommand;
	    private readonly ScriptDb _scriptDb;
	    private readonly MasterScriptDb _masterScriptDb;
		private string _gridVisibility;
	    private string _message;
	    private string _messageVisibility;
		private ObservableCollection<KeyValuePair<string,Script>> _scriptsCollection = new ObservableCollection<KeyValuePair<string, Script>>();
		private ObservableCollection<ImportScriptItemTemplate> _filesNameCollection = new ObservableCollection<ImportScriptItemTemplate>();

		public ImportScriptPageViewModel(MainWindowViewModel mainWindowViewModel)
		{
			_mainWindowViewModel = mainWindowViewModel;
		}

	    public ImportScriptPageViewModel()
	    {
		    _gridVisibility = "Hidden";
		    _messageVisibility = "Hidden";
		    _scriptDb = new ScriptDb();
		    _masterScriptDb = new MasterScriptDb();
		}
	    public ICommand BackCommand => _backCommand ?? (_backCommand = new CommandHandler(BackToScriptsList, true));

		public ICommand DragEnterCommand => _dragEnterCommand ??
											(_dragEnterCommand = new RelayCommand(HandlePreviewDrop));

	    public ICommand RemoveFileCommand => _removeFileCommand ?? (_removeFileCommand = new RelayCommand(RemoveFile));

	    public ICommand AddToMasterCommand => _addToMasterCommand ??
	                                          (_addToMasterCommand = new CommandHandler(ImportScriptsToMaster, true));
	    public ICommand ChangeScriptStateCommand => _changeScriptStateCommand ?? (_changeScriptStateCommand = new RelayCommand(ChangeState));

		private async void ImportScriptsToMaster()
	    {
		    var scriptsToBeImported = ScriptsCollection.Where(s => s.Value.IsSelected).Select(s =>s.Value).ToList();
		    if (scriptsToBeImported.Count > 0)
		    {
			    foreach (var script in scriptsToBeImported)
			    {
				    script.IsSelected = false;
				    script.ScriptStateAction = script.Active ? "Disable" : "Enable";
				    script.RowColor = script.Active ? "Black" : "DarkGray";
					await _scriptDb.AddNewScript(script);
			    }

			    var masterScript = await _masterScriptDb.GetMasterScript();
			    masterScript.Scripts.AddRange(scriptsToBeImported);
			    await _masterScriptDb.UpdateScript(masterScript);
			    //write masterscript on the disk
			    ProcessScript.ExportScript(Path.Combine(masterScript.Location, masterScript.Name), masterScript.Scripts);
			    Message = "Scripts imported successfully";
				MessageVisibility = "Visible";
		    }
		    else
		    {
				 MessageBox.Show("Please select at least one script from the grid to import", "Warning",
					MessageBoxButton.OK, MessageBoxImage.Warning);
			}

	    }

	    private void ChangeState(object row)
	    {
		    var script = ((KeyValuePair<string, Script>)row).Value;
		    if (script == null) return;
		    ProcessScript.SetStateColors(script);
		    ProcessScript.ChangeScriptState(script);

		    var scriptToBeUpdated = ScriptsCollection.FirstOrDefault(s => s.Value.ScriptId.Equals(script.ScriptId)).Value;
		    scriptToBeUpdated.Active = script.Active;
		    scriptToBeUpdated.Text = script.Text;
		    scriptToBeUpdated.RowColor = script.RowColor;
		    scriptToBeUpdated.ScriptStateAction = script.ScriptStateAction;
	    }

		private void RemoveFile(object file)
	    {
		    if (file != null)
		    {
			    var filePath = (string) file;
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
		    }
		    SetGridVisibility();
	    }

		private void HandlePreviewDrop(object dropedFile)
	    {
			var file = dropedFile as IDataObject;
		    if (null == file) return;
		    var documentsPath = (string[])file.GetData(DataFormats.FileDrop);
		    var defaultFormat = DataFormats.Text;

		    if (documentsPath != null)
		    {
				foreach (var path in documentsPath)
				{
					if (ProcessScript.IsGeneratedByAhkPlugin(path))
					{
						MessageVisibility = "Hidden";
						var pathAlreadyAdded = FilesNameCollection.Any(p => p.FilePath.Equals(path));
						if (!pathAlreadyAdded)
						{
							var scripts = ProcessScript.ReadImportedScript(path);
							foreach (var script in scripts)
							{
								script.Value.ScriptStateAction = script.Value.Active ? "Disable" : "Enable";
								script.Value.RowColor = script.Value.Active ? "Black" : "DarkGray";
							}

							ScriptsCollection.AddRange(scripts);
							var newFile = new ImportScriptItemTemplate
							{
								Content = Path.GetFileNameWithoutExtension(path),
								RemoveFileCommand = new RelayCommand(RemoveFile),
								FilePath = path
							};
							FilesNameCollection.Add(newFile);
						}
					}
					else
					{
						MessageVisibility = "Visible";
						Message = "Only scripts generated by AHK Plugin are supported.";
					}
				}
			}
		    SetGridVisibility();
	    }

	    private void SetGridVisibility()
	    {
			GridVisibility = ScriptsCollection.Count > 0 ? "Visible" : "Hidden";
		}
		private void BackToScriptsList()
	    {
		    _mainWindowViewModel.LoadScriptsPage();
	    }
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
	}
}

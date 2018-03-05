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
	    private string _gridVisibility;
		private ObservableCollection<KeyValuePair<string,Script>> _scriptsCollection = new ObservableCollection<KeyValuePair<string, Script>>();
		private ObservableCollection<ImportScriptItemTemplate> _filesNameCollection = new ObservableCollection<ImportScriptItemTemplate>();

		public ImportScriptPageViewModel(MainWindowViewModel mainWindowViewModel)
		{
			_mainWindowViewModel = mainWindowViewModel;
		}

	    public ImportScriptPageViewModel()
	    {
		    _gridVisibility = "Hidden";
	    }
	    public ICommand BackCommand => _backCommand ?? (_backCommand = new CommandHandler(BackToScriptsList, true));

		public ICommand DragEnterCommand => _dragEnterCommand ??
											(_dragEnterCommand = new RelayCommand(HandlePreviewDrop));

	    public ICommand RemoveFileCommand => _removeFileCommand ?? (_removeFileCommand = new RelayCommand(RemoveFile));

	    public ICommand AddToMasterCommand => _addToMasterCommand ??
	                                          (_addToMasterCommand = new CommandHandler(ImportScriptsToMaster, true));
	    public ICommand ChangeScriptStateCommand => _changeScriptStateCommand ?? (_changeScriptStateCommand = new RelayCommand(ChangeState));

		private void ImportScriptsToMaster()
	    {
		    var scriptsToBeImported = ScriptsCollection.Where(s => s.Value.IsSelected);
	    }

	    private void ChangeState(object row)
	    {

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
					var pathAlreadyAdded = FilesNameCollection.Any(p => p.FilePath.Equals(path));
					if (!pathAlreadyAdded)
					{
						var scripts = ProcessScript.ReadImportedScript(path);
						ScriptsCollection.AddRange(scripts);
						var newFile = new ImportScriptItemTemplate
						{
							Content =Path.GetFileNameWithoutExtension(path),
							RemoveFileCommand = new RelayCommand(RemoveFile),
							FilePath = path
						};
						FilesNameCollection.Add(newFile);
					}
				}
			}
		    SetGridVisibility();
		    // var test = File.ReadAllText(docPath[0]);
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
	}
}

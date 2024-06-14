using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SDLTM.Import.Command;
using SDLTM.Import.Helpers;
using SDLTM.Import.Interface;
using SDLTM.Import.Model;

namespace SDLTM.Import.ViewModel
{
	public class TmViewModel:WizardViewModelBase
    {
	    private int _currentPageNumber;
	    private string _displayName;
	    private string _tooltip;
	    private bool _isNextEnabled;
	    private bool _isPreviousEnabled;
	    private bool _includeSubfolders;
		private readonly WizardModel _wizardModel;
		private bool _isValid;
	    private ICommand _addfilesCommand;
	    private ICommand _dragEnterCommand;
	    private ICommand _removeFilesCommand;
	    private readonly ITmVmService _dataService;
	    private readonly IDialogService _dialogService;
	    private readonly string _filesFilter;
	    private readonly List<string> _filesExtensions;
	    public static readonly Log Log = Log.Instance;

		public TmViewModel(WizardModel wizardModel, ITmVmService dataService, IDialogService dialogService,
		    object view) : base(view)
	    {
		    _dataService = dataService;
		    _dialogService = dialogService;
		    _currentPageNumber = 1;
		    _wizardModel = wizardModel;
		    _displayName = PluginResources.Wizard_Select_TMs_and_Import_DisplayName; 
		    _tooltip = PluginResources.Wizard_Select_TM_View_Tooltip;
			_filesFilter = "SDL Xliff (*.sdlxliff) |*.sdlxliff|TMX|*.tmx";
		    _filesExtensions = new List<string> { "sdlxliff", "tmx"};
		    _wizardModel.TmsList.CollectionChanged += TmsListChanged;
		    _wizardModel.FilesList.CollectionChanged += FilesListChanged;
		    _isPreviousEnabled = true;
		    _includeSubfolders = false;
	    }

	    private void FilesListChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			IsValid=IsViewValid();
		}

		private void TmsListChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			IsValid = IsViewValid();
		}

		public override bool OnChangePage(int position, out string message)
		{
			message = string.Empty;

			var pagePosition = PageIndex - 1;
			if (position == pagePosition)
			{
				return false;
			}

			if (!IsValid && position > pagePosition)
			{
				message = PluginResources.Wizard_Select_ValidationMessage;
				return false;
			}
			return true;
		}

	    public ObservableCollection<TmDetails> TmsList
	    {
		    get => _wizardModel?.TmsList;
		    set
		    {
			    _wizardModel.TmsList = value;
			    OnPropertyChanged(nameof(TmsList));
		    }
	    }

	    public ObservableCollection<FileDetails> FilesList
	    {
		    get => _wizardModel?.FilesList;
		    set
		    {
			    _wizardModel.FilesList = value;
			    OnPropertyChanged(nameof(FilesList));
		    }
	    }

		public int CurrentPageNumber
	    {
		    get => _currentPageNumber;
		    set
		    {
			    _currentPageNumber = value;
			    OnPropertyChanged(nameof(CurrentPageNumber));
		    }
	    }

	    public override bool IsValid
	    {
		    get => _isValid;
		    set
		    {
			    if (_isValid == value)
				    return;

			    _isValid = value;
			    OnPropertyChanged(nameof(IsValid));
		    }
	    }

	    public bool IncludeSubfolders
	    {
		    get => _includeSubfolders;
		    set
		    {
			    if (_includeSubfolders == value)
			    {
				    return;
			    }
			    _includeSubfolders = value;
			    OnPropertyChanged(nameof(IncludeSubfolders));
		    }
	    }

	    public override string DisplayName
	    {
		    get => _displayName;
		    set
		    {
			    if (_displayName == value)
			    {
				    return;
			    }

			    _displayName = value;
			    OnPropertyChanged(nameof(DisplayName));
		    }
	    }

		public override string Tooltip
		{
			get => _tooltip;
			set
			{
				if (_tooltip == value) return;
				_tooltip = value;
				OnPropertyChanged(Tooltip);
			}
		}

		public bool IsNextEnabled
	    {
		    get => _isNextEnabled;
		    set
		    {
			    if (_isNextEnabled == value)
				    return;

			    _isNextEnabled = value;
			    OnPropertyChanged(nameof(IsNextEnabled));
		    }
	    }

	    public bool IsPreviousEnabled
	    {
		    get => _isPreviousEnabled;
		    set
		    {
			    if (_isPreviousEnabled == value)
				    return;

			    _isPreviousEnabled = value;
			    OnPropertyChanged(nameof(IsPreviousEnabled));
		    }
	    }

	    public ICommand AddFilesCommand => _addfilesCommand ?? (_addfilesCommand = new CommandHandler(AddFilesToGrid));
	    public ICommand DragEnterCommand => _dragEnterCommand ?? (_dragEnterCommand = new CommandHandler(HandleDrop));
	    public ICommand RemoveFilesCommand => _removeFilesCommand ?? (_removeFilesCommand = new CommandHandler(RemoveFiles));
		
	    private void RemoveFiles(object param)
	    {
			// We need to cast first to IList, we we try to cast directly to List<TmDetails> we'll receive an System.Windows.Coltrols.SelectedITemsCollection error
		    try
		    {
			    var listOfItems = (IList)param;
			    if (listOfItems is null) return;
			    var messgeBoxTitle = PluginResources.MeesageBox_Title;
				var messageBoxMessage = PluginResources.MessageBox_Delete_Conf_Msg;

				if (listOfItems[0]?.GetType() == typeof(TmDetails))
			    {
				    var result = _dialogService.ShowYesNoDialogResult(messgeBoxTitle, $"{messageBoxMessage} TM(s)?");
				    if (result == MessageDialogResult.Yes)
				    {
					    _dataService.RemoveTms(listOfItems, TmsList);
				    }
			    }
			    else
			    {
				    if (listOfItems[0]?.GetType() != typeof(FileDetails)) return;
				    {
					    var result = _dialogService.ShowYesNoDialogResult(messgeBoxTitle, $"{messageBoxMessage} files?");
					    if (result == MessageDialogResult.Yes)
					    {
							_dataService.RemoveFiles(listOfItems, FilesList);
						}
					}
			    }
		    }
		    catch (Exception e)
		    {
				Log.Logger.Error($"Remove files: {e.Message}\n {e.StackTrace}");
			}
		}

	    private void HandleDrop(object dropedFile)
	    {
		    var file = dropedFile as IDataObject;
		    var documentsPath = (string[]) file?.GetData(DataFormats.FileDrop);
		    if (documentsPath is null) return;
		    if (!documentsPath.Any()) return;
		    var tmsList = new List<string>();
		    var filesList = new List<string>();
		    foreach (var document in documentsPath)
		    {
			    var extension = Path.GetExtension(document);
			    if (!string.IsNullOrEmpty(extension))
			    {
				    if (extension.ToLower().Equals(".sdltm"))
				    {
					    tmsList.Add(document);
				    }
				    if (extension.ToLower().Equals(".sdlxliff") || extension.ToLower().Equals(".tmx"))
				    {
					    filesList.Add(document);
				    }
			    }
		    }
		    _dataService.AddFilesToGrid(filesList,FilesList);
		    _dataService.AddTmsToGrid(tmsList,TmsList);
	    }

	    private void AddFilesToGrid(object param)
	    {
		    if (param is null) return;
		    if (!Enum.TryParse(param.ToString(), true, out AddOptions option)) return;
		    switch (option)
		    {
			    case AddOptions.ChooseFilesFGrid:
					_dataService.AddFilesToGrid(SelectFiles(_filesFilter),FilesList);
				    break;
			    case AddOptions.ChooseTmTGrid:
					_dataService.AddTmsToGrid(SelectFiles("SDLTM(*.sdltm) | *.sdltm"),TmsList);
				    break;
			    case AddOptions.OpenFolderFGrid:
					_dataService.AddFilesToGrid(GetFilesFromFolder(_filesExtensions),FilesList);
				    break;
			    case AddOptions.OpenFolderTGrid:
					_dataService.AddTmsToGrid(GetFilesFromFolder(new List<string>{"sdltm"}),TmsList);
				    break;
		    }
	    }

	    private List<string> GetFilesFromFolder(List<string> fileExtensions)
	    {
		    var folderPath = _dialogService.ShowFolderDialog(PluginResources.Select_folderDialog_Title);
		    return _dataService.LoadFilesFromFolder(folderPath, fileExtensions,IncludeSubfolders);
	    }

		private List<string> SelectFiles(string filter)
	    {
		    return _dialogService.ShowFileDialog(filter);
	    }

	    private bool IsViewValid()
	    {
		    return TmsList.Count > 0 && FilesList.Count > 0;
	    }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.ApplyTMTemplate.Commands;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.Community.ApplyTMTemplate.Services;
using Sdl.Community.ApplyTMTemplate.Services.Interfaces;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.Versioning;
using Button = System.Windows.Controls.Button;

namespace Sdl.Community.ApplyTMTemplate.ViewModels
{
	public class MainWindowViewModel : ModelBase
	{
		private readonly FilePathDialogService _filePathDialogService;
		private readonly ILanguageResourcesAdapter _languageResourcesAdapter;
		private readonly IMessageService _messageService;
		private readonly IResourceManager _resourceManager;
		private readonly TmLoader _tmLoader;
		private ICommand _addFolderCommand;
		private ICommand _addTmsCommand;
		private ICommand _applyTemplateCommand;
		private ICommand _browseCommand;
		private bool _datesChecked;
		private ICommand _dragEnterCommand;
		private string _excelSheetPath;
		private ICommand _exportCommand;
		private ICommand _importCommand;
		private string _progressVisibility;
		private ICommand _removeTMsCommand;
		private TimedTextBox _timedTextBoxViewModel;
		private ObservableCollection<TranslationMemory> _tmCollection;
		private string _tmPath;

		public MainWindowViewModel(ILanguageResourcesAdapter languageResourcesAdapter, IResourceManager resourceManager, TmLoader tmLoader,
			IMessageService messageService, TimedTextBox timedTextBoxViewModel, FilePathDialogService filePathDialogService)
		{
			_languageResourcesAdapter = languageResourcesAdapter;
			_resourceManager = resourceManager;

			Settings = new Settings();
			Settings.PropertyChanged += Settings_PropertyChanged;

			_tmLoader = tmLoader;
			_messageService = messageService;
			_filePathDialogService = filePathDialogService;
			TimedTextBoxViewModel = timedTextBoxViewModel;

			ResourceTemplatePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				Versions.StudioDocumentsFolderName);

			_tmPath = ResourceTemplatePath;
			_excelSheetPath = ResourceTemplatePath;

			_progressVisibility = "Collapsed";

			_tmCollection = new ObservableCollection<TranslationMemory>();
		}

		public ICommand AddFolderCommand => _addFolderCommand ??= new CommandHandler(AddFolder, true);

		public ICommand AddTmCommand => _addTmsCommand ??= new CommandHandler(AddTms, true);

		public bool AllTmsChecked
		{
			get => AreAllTmsSelected();
			set
			{
				ToggleCheckAllTms(value);
				OnPropertyChanged(nameof(AllTmsChecked));
			}
		}

		public ICommand ApplyTemplateCommand => _applyTemplateCommand ??= new RelayCommand(
			a => ApplyTmTemplate(),
			p => SelectedTmsList.Count > 0);

		public bool DatesChecked
		{
			get => _datesChecked;
			set
			{
				_datesChecked = value;
				OnPropertyChanged(nameof(DatesChecked));
			}
		}

		public ICommand DragEnterCommand => _dragEnterCommand ??= new RelayCommand(HandlePreviewDrop);

		public ICommand ExportCommand => _exportCommand ??= new CommandHandler(ExportToExcel, true);

		public ICommand ImportCommand => _importCommand ??= new RelayCommand(Import);

		public string ProgressVisibility
		{
			get => _progressVisibility;
			set
			{
				_progressVisibility = value;
				OnPropertyChanged(nameof(ProgressVisibility));
			}
		}

		public ICommand RemoveTMsCommand => _removeTMsCommand ??= new CommandHandler(RemoveTMs, true);

		public string ResourceTemplatePath
		{
			get => _timedTextBoxViewModel.Path;
			set
			{
				_timedTextBoxViewModel.Path = value;
				OnPropertyChanged(nameof(ResourceTemplatePath));
			}
		}

		public bool SelectAllChecked
		{
			get => AreAllOptionsChecked();
			set
			{
				foreach (var propertyInfo in Settings.GetType().GetProperties())
				{
					if (propertyInfo.CanWrite) propertyInfo.SetValue(Settings, value);
				}

				OnPropertyChanged(nameof(SelectAllChecked));
			}
		}

		public List<TranslationMemory> SelectedTmsList
		{
			get => TmCollection.Where(tm => tm.IsSelected).ToList();
		}

		public Settings Settings { get; }

		public TimedTextBox TimedTextBoxViewModel
		{
			get => _timedTextBoxViewModel;
			set
			{
				_timedTextBoxViewModel = value;
				_timedTextBoxViewModel.BrowseCommand = BrowseCommand;
				_timedTextBoxViewModel.ImportCommand = ImportCommand;
				_timedTextBoxViewModel.ExportCommand = ExportCommand;
			}
		}

		public ObservableCollection<TranslationMemory> TmCollection
		{
			get => _tmCollection;
			set
			{
				_tmCollection = value;
				OnPropertyChanged(nameof(TmCollection));
			}
		}

		private ICommand BrowseCommand => _browseCommand ??= new CommandHandler(Browse, true);

		public bool IsTemplatePathValid()
		{
			if (string.IsNullOrWhiteSpace(ResourceTemplatePath))
			{
				_messageService.ShowErrorMessage(PluginResources.Error_Window_Title, PluginResources.No_resource_template_path_provided);
				return false;
			}

			try
			{
				_languageResourcesAdapter.Load(ResourceTemplatePath);
			}
			catch (Exception e)
			{
				_messageService.ShowWarningMessage(PluginResources.Template_loading_error, PluginResources.Please_specify_a_correct_template_path);
				return false;
			}

			return true;
		}

		private void AddFolder()
		{
			var filesPaths =
				_filePathDialogService.GetFilesFromFolderInputByUser(
					PluginResources.Please_select_the_folder_containing_the_TMs, _tmPath);

			if (filesPaths is null || !filesPaths.Any()) return;

			_tmPath = filesPaths[0];
			AddRangeOfTms(_tmLoader.GetTms(filesPaths, TmCollection));
		}

		private void AddRangeOfTms(IEnumerable<TranslationMemory> tmsCollection)
		{
			foreach (var tm in tmsCollection)
			{
				TmCollection.Add(tm);
				tm.PropertyChanged += Tm_PropertyChanged;
			}
		}

		private void AddTms()
		{
			var filePaths = _filePathDialogService.GetFilePathInputFromUser(filter: "Translation Memories|*.sdltm", initialDirectory: _tmPath, multiselect: true);
			if (filePaths is null || !filePaths.Any()) return;
			_tmPath = filePaths[0];
			AddRangeOfTms(_tmLoader.GetTms(filePaths, TmCollection));
		}

		private async void ApplyTmTemplate()
		{
			if (!IsTemplatePathValid()) return;
			UnmarkTms(SelectedTmsList);
			if (SelectedTmsList.Count == 0)
			{
				_messageService.ShowWarningMessage(PluginResources.Warning,
					PluginResources.Select_at_least_one_TM);
				return;
			}

			ProgressVisibility = "Visible";
			await Task.Run(() => _resourceManager.ApplyTemplateToTms(_languageResourcesAdapter, SelectedTmsList, Settings));
			ProgressVisibility = "Collapsed";
		}

		private bool AreAllOptionsChecked() => Settings.GetType().GetProperties().All(propertyInfo => !propertyInfo.CanWrite || (bool)propertyInfo.GetValue(Settings));

		private bool AreAllTmsSelected()
		{
			return TmCollection.Count > 0 && TmCollection.All(tm => tm.IsSelected);
		}

		private void Browse()
		{
			var resourceTemplatePath = _filePathDialogService.GetFilePathInputFromUser(initialDirectory: ResourceTemplatePath, filter: "Language resource templates|*.sdltm.resource");
			if (resourceTemplatePath == null) return;
			ResourceTemplatePath = resourceTemplatePath[0];
		}

		private async void ExportToExcel()
		{
			if (!IsTemplatePathValid()) return;

			var saveLocation = GetSaveLocation();
			if (saveLocation == null) return;

			ProgressVisibility = "Visible";
			await Task.Run(() =>
			{
				_resourceManager.ExportResourcesToExcel(_languageResourcesAdapter, saveLocation, Settings);
			});

			_messageService.ShowMessage(PluginResources.Success_Window_Title, PluginResources.Report_generated_successfully);
			Process.Start(saveLocation);

			ProgressVisibility = "Collapsed";
		}

		private string GetSaveLocation()
		{
			var saveLocation = "";
			try
			{
				_filePathDialogService.GetSaveLocationInputFromUser(out saveLocation,
					PluginResources.Export_language_resources, "Excel |*.xlsx", PluginResources.Exported_filename);
			}
			catch (Exception e)
			{
				_messageService.ShowWarningMessage(PluginResources.Warning,
					$"{e.Message}\n\n{PluginResources.A_new_file_created}: {saveLocation}");
			}

			return saveLocation;
		}

		private void HandlePreviewDrop(object droppedFile)
		{
			if (droppedFile == null) return;
			AddRangeOfTms(_tmLoader.GetTms(droppedFile as string[], TmCollection));
		}

		private async void Import(object parameter)
		{
			if (!IsTemplatePathValid()) return;
			try
			{
				if ((parameter as Button)?.Name == "ImportFromExcel")
				{
					var fileNamesInput = _filePathDialogService.GetFilePathInputFromUser(
						PluginResources.Import_window_title,
						filter: "Excel spreadsheet|*.xlsx|Translation memories|*.sdltm|Both|*.sdltm;*.xlsx",
						initialDirectory: _excelSheetPath);
					if (fileNamesInput == null) return;
					_excelSheetPath = fileNamesInput[0];
					await ImportResourcesFromExcel(fileNamesInput);
				}
				else
				{
					ProgressVisibility = "Collapsed";
					if (SelectedTmsList.Count > 0)
					{
						await ImportResourcesFromSdltm();
					}
					else
					{
						_messageService.ShowWarningMessage(PluginResources.Warning, PluginResources.Select_at_least_one_TM);
					}
					ProgressVisibility = "Collapsed";
				}
			}
			catch (Exception e)
			{
				_messageService.ShowErrorMessage(PluginResources.Error_Window_Title, e.Message);
				ProgressVisibility = "Collapsed";
			}
		}

		private async Task ImportResourcesFromExcel(string[] fileName)
		{
			ProgressVisibility = "Visible";
			await Task.Run(() =>
			{
				if (fileName[0].Contains(".xlsx"))
				{
					_resourceManager.ImportResourcesFromExcel(fileName[0], _languageResourcesAdapter, Settings);
				}
			});
			_messageService.ShowMessage(PluginResources.Success_Window_Title, PluginResources.Resources_Imported_Successfully);
			ProgressVisibility = "Collapsed";
		}

		private async Task ImportResourcesFromSdltm()
		{
			await Task.Run(() => { _resourceManager.ImportResourcesFromSdltm(SelectedTmsList, _languageResourcesAdapter, Settings); });
			_messageService.ShowMessage(PluginResources.Success_Window_Title, PluginResources.Resources_Imported_Successfully);
		}

		private void RemoveTMs()
		{
			TmCollection = new ObservableCollection<TranslationMemory>(TmCollection.Where(tm => !tm.IsSelected));
			OnPropertyChanged(nameof(AllTmsChecked));
		}

		private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnPropertyChanged(nameof(SelectAllChecked));
		}

		private void Tm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "IsSelected") return;
			OnPropertyChanged(nameof(AllTmsChecked));
		}

		private void ToggleCheckAllTms(bool onOff)
		{
			foreach (var translationMemory in TmCollection)
			{
				if (translationMemory.IsSelected != onOff)
				{
					translationMemory.IsSelected = onOff;
				}
			}
		}

		private void UnmarkTms(IEnumerable<TranslationMemory> tms)
		{
			foreach (var tm in tms)
			{
				tm.ResetAnnotations();
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.ApplyTMTemplate.Commands;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Sdl.Community.ApplyTMTemplate.ViewModels
{
	public class MainWindowViewModel : ModelBase
	{
		private readonly TemplateLoader _templateLoader;
		private readonly TMLoader _tmLoader;
		private readonly IDialogCoordinator _dialogCoordinator;
		private bool _abbreviationsChecked;
		private bool _ordinalFollowersChecked;
		private bool _segmentationRulesChecked;
		private bool _variablesChecked;
		private bool _allTmsChecked;
		private bool _toggleExcelTM;
		private bool _templateValidWithResources;
		private bool _templateValidNoResources;
		private bool _toggleDirection;

		private string _tmPath;
		private string _message;
		private string _progressVisibility;
		private string _unIDedLanguagesAsString;

		private ICommand _addFolderCommand;
		private ICommand _addTmsCommand;
		private ICommand _applyTemplateCommand;
		private ICommand _browseCommand;
		private ICommand _exportCommand;
		private ICommand _importCommand;
		private ICommand _dragEnterCommand;
		private ICommand _removeTMsCommand;
		private ObservableCollection<TranslationMemory> _tmCollection;
		private FileBasedLanguageResourcesTemplate _template;
		private List<int> _unIDedLanguages;

		private ExcelImportExportService _importExportService;
		private TimedTextBox _timedTextBoxViewModel;

		public MainWindowViewModel(TemplateLoader templateLoader, TMLoader tmLoader, IDialogCoordinator dialogCoordinator, TimedTextBox timedTextBoxViewModel)
		{
			_templateLoader = templateLoader;
			_tmLoader = tmLoader;
			_dialogCoordinator = dialogCoordinator;
			TimedTextBoxViewModel = timedTextBoxViewModel;

			_tmPath = _templateLoader.GetTmFolderPath();

			_variablesChecked = true;
			_abbreviationsChecked = true;
			_ordinalFollowersChecked = true;
			_segmentationRulesChecked = true;
			_progressVisibility = "Hidden";

			_tmCollection = new ObservableCollection<TranslationMemory>();

			_importExportService = new ExcelImportExportService();
		}

		public TimedTextBox TimedTextBoxViewModel
		{
			get => _timedTextBoxViewModel;
			set
			{
				_timedTextBoxViewModel = value;
				_timedTextBoxViewModel.ShouldStartValidation += StartLoadingResourcesAndValidate;
			}
		}

		public async void StartLoadingResourcesAndValidate(object sender, EventArgs e)
		{
			LoadResourcesFromTemplate();

			_templateValidWithResources = await ValidateTemplateAndShowErrors();
			_templateValidNoResources = await ValidateTemplateAndShowErrors(false);

			OnPropertyChanged(nameof(CanExecuteApply));
			OnPropertyChanged(nameof(CanExecuteImport));
			OnPropertyChanged(nameof(CanExecuteExport));
		}

		public string ProgressVisibility
		{
			get => _progressVisibility;
			set
			{
				_progressVisibility = value;
				OnPropertyChanged(nameof(ProgressVisibility));
			}
		}

		public bool ToggleExcelTM
		{
			get => _toggleExcelTM;
			set
			{
				_toggleExcelTM = value;
				OnPropertyChanged(nameof(ToggleExcelTM));
				OnPropertyChanged(nameof(CanExecuteImport));
			}
		}

		public bool AbbreviationsChecked
		{
			get => _abbreviationsChecked;
			set
			{
				_abbreviationsChecked = value;
				OnPropertyChanged(nameof(AbbreviationsChecked));
			}
		}

		public bool OrdinalFollowersChecked
		{
			get => _ordinalFollowersChecked;
			set
			{
				_ordinalFollowersChecked = value;
				OnPropertyChanged(nameof(OrdinalFollowersChecked));
			}
		}

		public bool SegmentationRulesChecked
		{
			get => _segmentationRulesChecked;
			set
			{
				_segmentationRulesChecked = value;
				OnPropertyChanged(nameof(SegmentationRulesChecked));
			}
		}

		public bool VariablesChecked
		{
			get => _variablesChecked;
			set
			{
				_variablesChecked = value;
				OnPropertyChanged(nameof(VariablesChecked));
			}
		}

		public bool CanExecuteExport => _templateValidWithResources;

		public string ResourceTemplatePath
		{
			get => _timedTextBoxViewModel.Path;
			set
			{
				_timedTextBoxViewModel.Path = value;
				OnPropertyChanged(nameof(ResourceTemplatePath));
			}
		}

		public bool AllTmsChecked
		{
			get => _allTmsChecked;
			set
			{
				if (value)
				{
					ToggleCheckAllTms(true);
				}

				_allTmsChecked = value;
				OnPropertyChanged(nameof(AllTmsChecked));
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

		public ICommand AddFolderCommand => _addFolderCommand ?? (_addFolderCommand = new CommandHandler(AddFolder, true));

		public ICommand AddTmCommand => _addTmsCommand ?? (_addTmsCommand = new CommandHandler(AddTms, true));

		public ICommand ApplyTemplateCommand => _applyTemplateCommand ?? (_applyTemplateCommand = new CommandHandler(ApplyTmTemplate, true));

		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new CommandHandler(Browse, true));

		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export,  true));

		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));

		public ICommand DragEnterCommand => _dragEnterCommand ?? (_dragEnterCommand = new RelayCommand(HandlePreviewDrop));

		public ICommand RemoveTMsCommand => _removeTMsCommand ?? (_removeTMsCommand = new CommandHandler(RemoveTMs, true));

		public bool ToggleDirection
		{
			get => _toggleDirection;
			set
			{
				_toggleDirection = value;
				OnPropertyChanged(nameof(ToggleDirection));
			}
		}

		private string CreateNewFile(string filePath)
		{
			var index = 0;
			while (true)
			{
				if (index == 0)
				{
					filePath = filePath.Insert(filePath.IndexOf(".xlsx", StringComparison.OrdinalIgnoreCase),
						$"_{index}");
				}
				else
				{
					filePath = filePath.Replace((index - 1).ToString(), index.ToString());
				}

				if (File.Exists(filePath))
				{
					index++;
					continue;
				}

				break;
			}

			return filePath;
		}

		private void Tm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsSelected")
			{
				if (!(sender is TranslationMemory translationMemorySender)) return;

				if (translationMemorySender.IsSelected && AreAllTmsSelectedOrUnselected())
				{
					AllTmsChecked = true;
				}

				if (translationMemorySender.IsSelected)
				{
					OnPropertyChanged(nameof(CanExecuteApply));
					OnPropertyChanged(nameof(CanExecuteImport));
				}

				if (!translationMemorySender.IsSelected && AreAllTmsSelectedOrUnselected())
				{
					OnPropertyChanged(nameof(CanExecuteApply));
					OnPropertyChanged(nameof(CanExecuteImport));
				}

				if (AllTmsChecked && !translationMemorySender.IsSelected)
				{
					AllTmsChecked = false;
				}
			}
		}

		private bool AreAllTmsSelectedOrUnselected()
		{
			for (var i = 0; i < TmCollection.Count - 1; i++)
			{
				if (TmCollection[i].IsSelected != TmCollection[i + 1].IsSelected) return false;
			}

			return true;
		}

		private void AddFolder()
		{
			var dlg = new FolderSelectDialog
			{
				Title = PluginResources.Please_select_the_folder_containing_the_TMs,
				InitialDirectory = _tmPath
			};

			if (!dlg.ShowDialog()) return;

			_tmPath = dlg.FileName;

			if (string.IsNullOrEmpty(_tmPath)) return;

			var files = Directory.GetFiles(_tmPath);

			AddRange(_tmLoader.GetTms(files, TmCollection));
		}

		private void AddRange(ObservableCollection<TranslationMemory> tmsCollection)
		{
			foreach (var tm in tmsCollection)
			{
				TmCollection.Add(tm);
				tm.PropertyChanged += Tm_PropertyChanged;
			}
		}

		private void AddTms()
		{
			var dlg = new OpenFileDialog()
			{
				Filter = "Translation Memories|*.sdltm",
				InitialDirectory = _tmPath,
				Multiselect = true
			};

			dlg.ShowDialog();
			AddRange(_tmLoader.GetTms(dlg.FileNames, TmCollection));
		}

		private void LoadResourcesFromTemplate()
		{
			var languageResourceBundles = _templateLoader.GetLanguageResourceBundlesFromFile(ResourceTemplatePath, out _message, out _unIDedLanguages);

			CreateTemplateObjectFromBundles(languageResourceBundles);
		}

		private void CreateTemplateObjectFromBundles(List<LanguageResourceBundle> languageResourceBundles)
		{
			_template = new FileBasedLanguageResourcesTemplate();

			if (languageResourceBundles == null) return;

			foreach (var bundle in languageResourceBundles)
			{
				_template.LanguageResourceBundles.Add(bundle);
			}
		}

		private async void ApplyTmTemplate()
		{
			LoadResourcesFromTemplate();

			if (!await ValidateTemplateAndShowErrors()) return;

			var selectedTms = TmCollection.Where(tm => tm.IsSelected).ToList();
			UnMarkTms(selectedTms);

			if (selectedTms.Count == 0)
			{
				await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Warning, PluginResources.Select_at_least_one_TM);
				return;
			}

			var template = new Template(_template);

			var settings = new Settings(AbbreviationsChecked, VariablesChecked, OrdinalFollowersChecked, SegmentationRulesChecked);

			ProgressVisibility = "Visible";
			await Task.Run(() => template.ApplyTmTemplate(selectedTms, settings));
			ProgressVisibility = "Hidden";
		}

		private async Task<bool> ValidateTemplateAndShowErrors(bool checkIfBundlesPresent = true)
		{
			var isValid = ValidateTemplate(checkIfBundlesPresent, out _unIDedLanguagesAsString);

			if (!string.IsNullOrEmpty(_unIDedLanguagesAsString) || !isValid)
			{
				await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Warning,
					_message);
			}

			return isValid;
		}

		private bool ValidateTemplate(bool checkIfBundlesPresent, out string unIDedLanguages)
		{
			var isValid = true;

			unIDedLanguages = _unIDedLanguages?.Aggregate("", (i, j) => i + "\n  \u2022" + j);

			if (checkIfBundlesPresent)
			{
				if (_template == null || _template.LanguageResourceBundles.Count == 0)
				{
					isValid = false;

					if (!string.IsNullOrEmpty(unIDedLanguages))
					{
						_message = $"{PluginResources.No_Languages_IDed}\n\n{PluginResources.Unidentified_Languages}{unIDedLanguages}";
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(unIDedLanguages))
					{
						var idedLanguages = _template.LanguageResourceBundles.Aggregate("", (l, j) => l + "\n  \u2022" + j.LanguageCode);
						_message = $"{PluginResources.Identified_Languages}{idedLanguages}" +
						           $"\n\n{PluginResources.Unidentified_Languages}{unIDedLanguages}";
					}
				}
			}
			else
			{
				if (_message == PluginResources.Template_corrupted_or_file_not_template)
				{
					isValid = false;
				}

			}

			return isValid;
		}

		public bool CanExecuteImport
		{
			get
			{
				if (!ToggleExcelTM && (_templateValidNoResources || _templateValidWithResources))
				{
					return true;
				}

				if (ToggleExcelTM && (_templateValidNoResources || _templateValidWithResources) &&
				    IsThereAnyTmSelected())
				{
					return true;
				}

				return false;
			}
		}

		private bool IsThereAnyTmSelected()
		{
			return TmCollection.Any(tm => tm.IsSelected);
		}

		public bool CanExecuteApply => _templateValidWithResources && IsThereAnyTmSelected();

		private void UnMarkTms(List<TranslationMemory> tms)
		{
			foreach (var tm in tms)
			{
				tm.UnmarkTm();
				tm.UnmarkTm();
			}
		}

		private async void Import()
		{
			LoadResourcesFromTemplate();

			if (!await ValidateTemplateAndShowErrors(false)) return;

			var settings = new Settings(AbbreviationsChecked, VariablesChecked, OrdinalFollowersChecked, SegmentationRulesChecked);

			if (!ToggleExcelTM)
			{
				var dlg = new OpenFileDialog()
				{
					Title = PluginResources.Import_window_title,
					Filter = "Excel spreadsheet|*.xlsx|Translation memories|*.sdltm|Both|*.sdltm;*.xlsx",
				};

				var result = dlg.ShowDialog();

				if (!(result ?? false)) return;

				ProgressVisibility = "Visible";
				try
				{
					await Task.Run(() =>
					{
						if (dlg.FileName.Contains(".xlsx"))
						{
							_importExportService.ImportResourcesFromExcel(dlg.FileName, ResourceTemplatePath, settings, _template);
						}
					});
				}
				catch (Exception e)
				{
					await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Error_Window_Title, e.Message);

					ProgressVisibility = "Hidden";
					return;
				}

				await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Success_Window_Title, PluginResources.Resources_Imported_Successfully);

				ProgressVisibility = "Hidden";
			}
			else
			{
				ProgressVisibility = "Visible";
				var selectedTmList = TmCollection.Where(tm => tm.IsSelected).ToList();
				if (selectedTmList.Count > 0)
				{
					try
					{
						await Task.Run(() =>
						{
							_importExportService.ImportResourcesFromSdltm(selectedTmList, settings, ResourceTemplatePath, _template);
						});
					}
					catch (Exception e)
					{
						await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Error_Window_Title, e.Message);

						ProgressVisibility = "Hidden";
						return;
					}

					await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Success_Window_Title, PluginResources.Resources_Imported_Successfully);
				}
				else
				{
					await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Warning, PluginResources.Select_at_least_one_TM);
				}

				ProgressVisibility = "Hidden";
			}
		}

		private async void Export()
		{
			LoadResourcesFromTemplate();

			if (!await ValidateTemplateAndShowErrors()) return;

			var settings = new Settings(AbbreviationsChecked, VariablesChecked, OrdinalFollowersChecked, SegmentationRulesChecked);

			var dlg = new SaveFileDialog
			{
				Title = PluginResources.Export_language_resources,
				Filter = @"Excel |*.xlsx",
				FileName = PluginResources.Exported_filename,
				AddExtension = false
			};

			var result = dlg.ShowDialog();

			if (result != DialogResult.OK) return;

			ProgressVisibility = "Visible";
			var filePath = dlg.FileName;

			if (File.Exists(filePath))
			{
				try
				{
					File.Delete(filePath);
				}
				catch (Exception e)
				{
					filePath = CreateNewFile(filePath);
					await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Warning, $"{e.Message}\n\n{PluginResources.A_new_file_created}: {filePath}");
				}
			}

			await Task.Run(() =>
			{
				_importExportService.ExportResources(_template, filePath, settings);
			});

			await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Success_Window_Title, PluginResources.Report_generated_successfully);
			Process.Start(filePath);

			ProgressVisibility = "Hidden";
		}

		private void Browse()
		{
			var dlg = new OpenFileDialog
			{
				Filter = "Language resource templates|*.sdltm.resource",
			};

			var result = dlg.ShowDialog();

			if (result == true)
			{
				ResourceTemplatePath = dlg.FileName;
			}
		}

		private void HandlePreviewDrop(object droppedFile)
		{
			if (droppedFile == null) return;

			AddRange(_tmLoader.GetTms(droppedFile as string[], TmCollection));
		}

		private void RemoveTMs()
		{
			TmCollection = new ObservableCollection<TranslationMemory>(TmCollection.Where(tm => !tm.IsSelected));

			if (TmCollection.Count == 0)
			{
				AllTmsChecked = false;
			}

			OnPropertyChanged(nameof(CanExecuteImport));
			OnPropertyChanged(nameof(CanExecuteApply));
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
	}
}
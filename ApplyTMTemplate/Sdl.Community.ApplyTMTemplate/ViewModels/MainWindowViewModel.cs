using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		private string _tmPath;
		private string _resourceTemplatePath;
		private string _message;
		private string _progressVisibility;

		private ICommand _addFolderCommand;
		private ICommand _addTmsCommand;
		private ICommand _applyTemplateCommand;
		private ICommand _browseCommand;
		private ICommand _exportCommand;
		private ICommand _importCommand;
		private ICommand _dragEnterCommand;
		private ICommand _removeTMsCommand;
		private ObservableCollection<TranslationMemory> _tmCollection;
		private List<LanguageResourceBundle> _langResBundlesList;
		private List<int> _unIDedLanguagess;

		private ExcelImportExportService _importExportService;

		public MainWindowViewModel(TemplateLoader templateLoader, TMLoader tmLoader, IDialogCoordinator dialogCoordinator)
		{
			_templateLoader = templateLoader;
			_tmLoader = tmLoader;
			_dialogCoordinator = dialogCoordinator;

			_importExportService = new ExcelImportExportService();

			_tmPath = _templateLoader.GetTmFolderPath();

			_variablesChecked = true;
			_abbreviationsChecked = true;
			_ordinalFollowersChecked = true;
			_segmentationRulesChecked = true;
			_progressVisibility = "Hidden";

			_tmCollection = new ObservableCollection<TranslationMemory>();

			_resourceTemplatePath = _templateLoader.GetTmTemplateFolderPath();

			LoadResourcesFromTemplate();
		}

		public string ProgressVisibility
		{
			get => _progressVisibility;
			set
			{
				_progressVisibility = value;
				OnPropertyChanged();
			}
		}

		public bool AbbreviationsChecked
		{
			get => _abbreviationsChecked;
			set
			{
				_abbreviationsChecked = value;
				OnPropertyChanged();
			}
		}

		public bool OrdinalFollowersChecked
		{
			get => _ordinalFollowersChecked;
			set
			{
				_ordinalFollowersChecked = value;
				OnPropertyChanged();
			}
		}

		public bool SegmentationRulesChecked
		{
			get => _segmentationRulesChecked;
			set
			{
				_segmentationRulesChecked = value;
				OnPropertyChanged();
			}
		}

		public bool VariablesChecked
		{
			get => _variablesChecked;
			set
			{
				_variablesChecked = value;
				OnPropertyChanged();
			}
		}

		public string ResourceTemplatePath
		{
			get => _resourceTemplatePath;
			set
			{
				if (_resourceTemplatePath != value)
				{
					_resourceTemplatePath = value;
					LoadResourcesFromTemplate();
					OnPropertyChanged(nameof(ResourceTemplatePath));
				}
			}
		}

		public bool AllTmsChecked
		{
			get => _allTmsChecked;
			set
			{
				ToggleCheckAllTms(value);

				_allTmsChecked = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<TranslationMemory> TmCollection
		{
			get => _tmCollection;
			set
			{
				_tmCollection = value;
				OnPropertyChanged();
			}
		}

		public ICommand AddFolderCommand => _addFolderCommand ?? (_addFolderCommand = new CommandHandler(AddFolder, true));

		public ICommand AddTmCommand => _addTmsCommand ?? (_addTmsCommand = new CommandHandler(AddTms, true));

		public ICommand ApplyTemplateCommand => _applyTemplateCommand ?? (_applyTemplateCommand = new CommandHandler(ApplyTmTemplate, true));

		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new CommandHandler(Browse, true));

		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export, true));

		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));

		public ICommand DragEnterCommand => _dragEnterCommand ?? (_dragEnterCommand = new RelayCommand(HandlePreviewDrop));

		public ICommand RemoveTMsCommand => _removeTMsCommand ?? (_removeTMsCommand = new CommandHandler(RemoveTMs, true));

		private void Tm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsSelected" && TmCollection[0].IsSelected && AreAllTmsSelectedOrUnselected())
			{
				AllTmsChecked = true;
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
			_langResBundlesList = _templateLoader.GetLanguageResourceBundlesFromFile(ResourceTemplatePath, out _message, out _unIDedLanguagess);
		}

		private async void ApplyTmTemplate()
		{
			ProgressVisibility = "Visible";

			if (_langResBundlesList == null || _langResBundlesList.Count == 0)
			{
				await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Warning_Window_Title, _message);
				return;
			}

			//transform the list of unIDedLanguages and idedLanguages into two strings, respectively
			var idedLanguages = _langResBundlesList.Aggregate("", (l, j) => l + "\n  \u2022" + j.LanguageCode);
			var unIDedLanguages = _unIDedLanguagess.Aggregate("", (i, j) => i + "\n  \u2022" + j);

			if (unIDedLanguages != "")
			{
				await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Warning_Window_Title,
					$"{PluginResources.Identified_Languages}{idedLanguages}" +
					$"\n\n{PluginResources.Unidentified_Languages}{unIDedLanguages}");
			}

			var selectedTmList = TmCollection.Where(tm => tm.IsSelected).ToList();
			UnMarkTms(selectedTmList);

			if (selectedTmList.Count == 0)
			{
				await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Warning_Window_Title, PluginResources.Select_at_least_one_TM);
				return;
			}

			var template = new Template(_langResBundlesList);

			var options = new Options
			{
				AbbreviationsChecked = AbbreviationsChecked,
				VariablesChecked = VariablesChecked,
				OrdinalFollowersChecked = OrdinalFollowersChecked,
				SegmentationRulesChecked = SegmentationRulesChecked
			};

			await Task.Run(() => template.ApplyTmTemplate(selectedTmList, options));

			ProgressVisibility = "Hidden";
		}

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
			var dlg = new OpenFileDialog()
			{
				Title = PluginResources.Import_window_title,
				Filter = "Excel spreadsheet|*.xlsx|Translation memories|*.sdltm|Both|*.sdltm;*.xlsx",
				InitialDirectory = ResourceTemplatePath.Contains(".")
					? ResourceTemplatePath.Substring(0, ResourceTemplatePath.LastIndexOf('\\') + 1)
					: ResourceTemplatePath
			};

			var result = dlg.ShowDialog();

			if (!(result ?? false)) return;

			ProgressVisibility = "Visible";
			await Task.Run(() =>
			{
				if (dlg.FileName.Contains(".xlsx"))
				{
					try
					{
						_importExportService.ImportResources(dlg.FileName, ResourceTemplatePath,
							new Settings(AbbreviationsChecked, VariablesChecked, OrdinalFollowersChecked,
								SegmentationRulesChecked));
					}
					catch (Exception e)
					{
						_dialogCoordinator.ShowMessageAsync(this, PluginResources.Error_Window_Title,
							e.Message);
					}
				}

				if (dlg.FileName.Contains(".sdltm"))
				{
				}
			});
			ProgressVisibility = "Hidden";
		}

		private async void Export()
		{
			if (_langResBundlesList == null || _langResBundlesList.Count == 0)
			{
				await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Warning_Window_Title, _message);
				return;
			}

			//transform the list of unIDedLanguages and idedLanguages into two strings, respectively
			var idedLanguages = _langResBundlesList.Aggregate("", (l, j) => l + "\n  \u2022" + j.LanguageCode);
			var unIDedLanguages = _unIDedLanguagess.Aggregate("", (i, j) => i + "\n  \u2022" + j);

			if (unIDedLanguages != "")
			{
				await _dialogCoordinator.ShowMessageAsync(this, PluginResources.Warning_Window_Title,
					$"{PluginResources.Identified_Languages}{idedLanguages}" +
					$"\n\n{PluginResources.Unidentified_Languages}{unIDedLanguages}");
			}

			var dlg = new SaveFileDialog
			{
				Title = PluginResources.Export_language_resources,
				Filter = @"Excel |*.xlsx",
				FileName = "Exported resources",
				AddExtension = true
			};

			var result = dlg.ShowDialog();

			if (result == DialogResult.OK)
			{
				ProgressVisibility = "Visible";
				var filePath = dlg.FileName;
				await Task.Run(() =>
				{
					_importExportService.ExportResources(_langResBundlesList, filePath);
				});
				ProgressVisibility = "Hidden";
			}
		}

		private void Browse()
		{
			var dlg = new OpenFileDialog
			{
				Filter = "Language resource templates|*.resource",
				InitialDirectory = ResourceTemplatePath.Contains(".")
					? ResourceTemplatePath.Substring(0, ResourceTemplatePath.LastIndexOf('\\') + 1)
					: ResourceTemplatePath
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
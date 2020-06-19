using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.ApplyTMTemplate.Commands;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Services;
using Sdl.Community.ApplyTMTemplate.Services.Interfaces;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Button = System.Windows.Controls.Button;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Sdl.Community.ApplyTMTemplate.ViewModels
{
	public class MainWindowViewModel : ModelBase
	{
		private readonly IMessageService _messageService;
		private readonly TemplateLoader _templateLoader;
		private readonly TMLoader _tmLoader;
		private bool _abbreviationsChecked;
		private ICommand _addFolderCommand;
		private ICommand _addTmsCommand;
		private ICommand _applyTemplateCommand;
		private ICommand _browseCommand;
		private ICommand _dragEnterCommand;
		private ICommand _exportCommand;
		private ICommand _importCommand;
		private string _message;
		private bool _ordinalFollowersChecked;
		private string _progressVisibility;
		private ICommand _removeTMsCommand;
		private ResourceManager _resourceManager;
		private bool _segmentationRulesChecked;
		private bool _selectAllChecked;
		private TemplateValidity _templateValidity;
		private TimedTextBox _timedTextBoxViewModel;
		private ObservableCollection<TranslationMemory> _tmCollection;
		private string _tmPath;
		private bool _variablesChecked;

		public MainWindowViewModel(TemplateLoader templateLoader, TMLoader tmLoader,
			IMessageService messageService, TimedTextBox timedTextBoxViewModel)
		{
			_templateLoader = templateLoader;
			_tmLoader = tmLoader;
			_messageService = messageService;
			TimedTextBoxViewModel = timedTextBoxViewModel;

			_tmPath = _templateLoader.GetTmFolderPath();

			_variablesChecked = true;
			_abbreviationsChecked = true;
			_ordinalFollowersChecked = true;
			_segmentationRulesChecked = true;
			_selectAllChecked = true;
			_progressVisibility = "Collapsed";

			_tmCollection = new ObservableCollection<TranslationMemory>();
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

		public ICommand AddFolderCommand => _addFolderCommand ??= new CommandHandler(AddFolder, true);

		public ICommand AddTmCommand => _addTmsCommand ??= new CommandHandler(AddTms, true);

		public bool AllTmsChecked
		{
			get => AreAllTmsSelected();
			set
			{
				if (value)
				{
					ToggleCheckAllTms(true);
				}

				OnPropertyChanged(nameof(AllTmsChecked));
			}
		}

		public ICommand ApplyTemplateCommand => _applyTemplateCommand ??= new RelayCommand(
			ao => ApplyTmTemplate(),
			po => (int)_templateValidity > 1 && SelectedTmsList.Count > 0);

		public bool CanExecuteExport => _templateValidity.HasFlag(TemplateValidity.HasResources);

		public ICommand DragEnterCommand => _dragEnterCommand ??= new RelayCommand(HandlePreviewDrop);

		public ICommand ExportCommand => _exportCommand ??= new CommandHandler(Export, true);

		public ICommand ImportCommand => _importCommand ??= new RelayCommand(Import);

		public bool OrdinalFollowersChecked
		{
			get => _ordinalFollowersChecked;
			set
			{
				_ordinalFollowersChecked = value;
				OnPropertyChanged(nameof(OrdinalFollowersChecked));
			}
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

		public bool SegmentationRulesChecked
		{
			get => _segmentationRulesChecked;
			set
			{
				_segmentationRulesChecked = value;
				OnPropertyChanged(nameof(SegmentationRulesChecked));
			}
		}

		public bool SelectAllChecked
		{
			get => _selectAllChecked;
			set
			{
				_selectAllChecked = value;
				AbbreviationsChecked = value;
				VariablesChecked = value;
				OrdinalFollowersChecked = value;
				SegmentationRulesChecked = value;
				OnPropertyChanged(nameof(SelectAllChecked));
			}
		}

		public List<TranslationMemory> SelectedTmsList
		{
			get { return TmCollection.Where(tm => tm.IsSelected).ToList(); }
		}

		public TimedTextBox TimedTextBoxViewModel
		{
			get => _timedTextBoxViewModel;
			set
			{
				_timedTextBoxViewModel = value;
				_timedTextBoxViewModel.BrowseCommand = BrowseCommand;
				_timedTextBoxViewModel.ImportCommand = ImportCommand;
				_timedTextBoxViewModel.ExportCommand = ExportCommand;
				_timedTextBoxViewModel.ShouldStartValidation += StartLoadingResourcesAndValidate;
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

		public bool VariablesChecked
		{
			get => _variablesChecked;
			set
			{
				_variablesChecked = value;
				OnPropertyChanged(nameof(VariablesChecked));
			}
		}

		private ICommand BrowseCommand => _browseCommand ??= new CommandHandler(Browse, true);

		public void StartLoadingResourcesAndValidate(object sender, EventArgs e)
		{
			_templateValidity = TemplateValidity.IsNotValid;

			LoadResourcesFromTemplate();

			//check if the template is valid(resources ignored)
			if (ValidateTemplate(false))
			{
				_templateValidity = TemplateValidity.IsValid;
			}
			//check if the template has any resources
			if (ValidateTemplate())
			{
				_templateValidity = TemplateValidity.IsValid | TemplateValidity.HasResources;
			}

			OnPropertyChanged(nameof(CanExecuteExport));

			ShowMessages();
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

		private async void ApplyTmTemplate()
		{
			var isValid = ValidateTemplate();
			ShowMessages();

			if (!isValid) return;

			UnmarkTms(SelectedTmsList);

			if (SelectedTmsList.Count == 0)
			{
				_messageService.ShowWarningMessage(PluginResources.Warning,
					PluginResources.Select_at_least_one_TM);
				return;
			}

			ProgressVisibility = "Visible";
			await Task.Run(() => _resourceManager.ApplyTemplateToTms(SelectedTmsList));
			ProgressVisibility = "Collapsed";
		}

		private bool AreAllTmsSelected()
		{
			return TmCollection.Count > 0 && TmCollection.All(tm => tm.IsSelected);
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

		private async void Export()
		{
			LoadResourcesFromTemplate();

			var isValid = ValidateTemplate();
			ShowMessages();
			if (!isValid) return;

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
					_messageService.ShowWarningMessage(PluginResources.Warning, $"{e.Message}\n\n{PluginResources.A_new_file_created}: {filePath}");
				}
			}

			await Task.Run(() =>
			{
				_resourceManager.ExportResourcesToExcel(filePath, settings);
			});

			_messageService.ShowMessage(PluginResources.Success_Window_Title, PluginResources.Report_generated_successfully);
			Process.Start(filePath);

			ProgressVisibility = "Collapsed";
		}

		private void HandlePreviewDrop(object droppedFile)
		{
			if (droppedFile == null) return;

			AddRange(_tmLoader.GetTms(droppedFile as string[], TmCollection));
		}

		private async void Import(object parameter)
		{
			var isValid = ValidateTemplate(false);
			if (!isValid) return;

			try
			{
				if ((parameter as Button)?.Name == "ImportFromExcel")
				{
					var dlg = new OpenFileDialog()
					{
						Title = PluginResources.Import_window_title,
						Filter = "Excel spreadsheet|*.xlsx|Translation memories|*.sdltm|Both|*.sdltm;*.xlsx",
					};

					var result = dlg.ShowDialog();

					if (!(result ?? false)) return;

					ProgressVisibility = "Visible";
					await Task.Run(() =>
					{
						if (dlg.FileName.Contains(".xlsx"))
						{
							_resourceManager.ImportResourcesFromExcel(dlg.FileName);
						}
					});
					_messageService.ShowMessage(PluginResources.Success_Window_Title, PluginResources.Resources_Imported_Successfully);

					ProgressVisibility = "Collapsed";
				}
				else
				{
					ProgressVisibility = "Collapsed";

					if (SelectedTmsList.Count > 0)
					{
						await Task.Run(() =>
						{
							_resourceManager.ImportResourcesFromSdltm(SelectedTmsList);
						});
						_messageService.ShowMessage(PluginResources.Success_Window_Title, PluginResources.Resources_Imported_Successfully);
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
				return;
			}

			_templateValidity = TemplateValidity.HasResources;
			OnPropertyChanged(nameof(CanExecuteExport));
		}

		private void LoadResourcesFromTemplate()
		{
			_resourceManager = new ResourceManager(new Settings(AbbreviationsChecked, VariablesChecked, OrdinalFollowersChecked, SegmentationRulesChecked), new ExcelResourceManager(), new LanguageResourcesTemplateContainer(ResourceTemplatePath));
		}

		private void RemoveTMs()
		{
			TmCollection = new ObservableCollection<TranslationMemory>(TmCollection.Where(tm => !tm.IsSelected));

			OnPropertyChanged(nameof(AllTmsChecked));
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

		private void UnmarkTms(List<TranslationMemory> tms)
		{
			foreach (var tm in tms)
			{
				tm.UnmarkTm();
			}
		}

		private bool ValidateTemplate(bool checkIfBundlesPresent = true)
		{
			//TODO: refactor this method; we shouldn't validate based upon the error message
			var isValid = true;
			if (_resourceManager != null)
			{
				if (_resourceManager.LanguageResourceBundles != null && checkIfBundlesPresent)
				{
					if (_resourceManager.LanguageResourceBundles.Count == 0)
					{
						isValid = false;

						
					}
				}
				else
				{
					if (_message == PluginResources.Template_corrupted_or_file_not_template ||
						_message == PluginResources.Template_filePath_Not_Correct)
					{
						isValid = false;
					}
				}
			}
			else
			{
				if (string.IsNullOrEmpty(ResourceTemplatePath))
				{
					_message = PluginResources.Select_A_Template;
				}
				isValid = false;
			}

			return isValid;
		}
	}
}
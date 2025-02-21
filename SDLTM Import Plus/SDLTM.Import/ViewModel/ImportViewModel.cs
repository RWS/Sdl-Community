using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Sdl.LanguagePlatform.TranslationMemory;
using SDLTM.Import.Command;
using SDLTM.Import.Helpers;
using SDLTM.Import.Interface;
using SDLTM.Import.Model;
using UserControl = System.Windows.Controls.UserControl;

namespace SDLTM.Import.ViewModel
{
	public class ImportViewModel : WizardViewModelBase
	{
		private int _currentPageNumber;
		private string _displayName;
		private bool _isNextEnabled;
		private bool _isPreviousEnabled;
		private bool _isValid;
		private bool _isImportBtnEnabled;
		private string _tooltip;
		private string _emptyGridMessageVisibility;
		private ICommand _importFilesCommand;
		private readonly ITranslationMemoryService _translationMemoryService;
		private readonly WizardModel _wizardModel;
		private readonly UserControl _view;
		private readonly IImportViewModelService _importVmService;

		public ImportViewModel(WizardModel wizardModel, ITranslationMemoryService translationMemoryService,IImportViewModelService importVmService, object view) :
			base(view)
		{
			_wizardModel = wizardModel;
			_currentPageNumber = 5;
			_isPreviousEnabled = true;
			_displayName = PluginResources.Wizard_FourthPage_DisplayName;
			_tooltip = PluginResources.Wizard_FourthPage_Tooltip;
			_isNextEnabled = false;
			_isImportBtnEnabled = true;
			_view = (UserControl) view;
			PropertyChanged += ImportViewModelChanged;
			_translationMemoryService = translationMemoryService;
			_importVmService = importVmService;
			if (_wizardModel?.ImportCollection != null) return;
			if (_wizardModel != null) _wizardModel.ImportCollection = new ObservableCollection<Model.Import>();
		}

		public override bool OnChangePage(int position, out string message)
		{
			message = string.Empty;
			return true;
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

		public string EmptyGridMessageVisibility
		{
			get => _emptyGridMessageVisibility;
			set
			{
				if(_emptyGridMessageVisibility == value)return;
				_emptyGridMessageVisibility = value;
				OnPropertyChanged(nameof(EmptyGridMessageVisibility));
			}
		}

		public bool IsImportBtnEnabled
		{
			get => _isImportBtnEnabled;
			set
			{
				_isImportBtnEnabled = value;
				OnPropertyChanged(nameof(IsImportBtnEnabled));
			}
		}

		public ObservableCollection<Model.Import> ImportCollection
		{
			get => _wizardModel?.ImportCollection;
			set
			{
				_wizardModel.ImportCollection = value;
				OnPropertyChanged(nameof(ImportCollection));
			}
		}

		private void ImportViewModelChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(CurrentPageChanged)) return;
			if (!IsCurrentPage) return;
			_wizardModel?.ImportCollection?.Clear();
			var groupedTms = _wizardModel?.TmsList.GroupBy(t => new {t.SourceLanguage, t.TargetLanguage}).ToList();
			if (groupedTms == null) return;
			foreach (var tm in groupedTms)
			{
				var importModel=_importVmService.GetImportModel(_wizardModel, tm.ToList(),tm.Key.SourceLanguage, tm.Key.TargetLanguage);
				ImportCollection.Add(importModel);
			}
			var isImportAvailable = _importVmService.IsImportAvailable(ImportCollection);
			IsImportBtnEnabled = isImportAvailable;
			EmptyGridMessageVisibility = isImportAvailable ? "Collapsed" : "Visible";
		}

		public ICommand ImportFilesCommand => _importFilesCommand ?? (_importFilesCommand = new CommandHandler(ImportTms));

		private async void ImportTms(object obj)
		{
			_translationMemoryService.Settings = _wizardModel?.ImportSettings;

			IsImportBtnEnabled = false;
			BackupTms();
			foreach (var import in ImportCollection)
			{
				foreach (var file in import.FilesCollection)
				{
					file.ImportStarted = true;
					foreach (var tm in import.TmsCollection)
					{
						tm.IsImportSummaryVisible = true;
						switch (file.FileType)
						{
							case FileTypes.Xliff:
								await ImportXliffFile(file, tm);
								break;
							case FileTypes.Tmx:
								await ImportTmxFile(file, tm);
								break;
						}
					}
					file.ImportStarted = false;
					file.ImportCompleted = true;
				}
			}
			_view?.Dispatcher?.Invoke(delegate { SendKeys.SendWait("{TAB}"); }, DispatcherPriority.ApplicationIdle);
			IsComplete = true;
		}

		private Task ImportTmxFile(FileDetails file, TmDetails tmDetails)
		{
			return Task.Factory.StartNew(() =>
			{
				if (tmDetails.HasCustomFiledsSelected())
				{
					_importVmService.ProcessTmxFile(file,tmDetails, _translationMemoryService);
				}
				else
				{
					var importSummary=_translationMemoryService.ImportFile(tmDetails.TranslationMemory,  file.Path);
					SetImportSummary(tmDetails, importSummary);
				}
			});
		}

		private Task ImportXliffFile(FileDetails fileDetails, TmDetails tmDetails)
		{
			return Task.Factory.StartNew(() =>
			{
				if (tmDetails.HasCustomFiledsSelected())
				{
					_importVmService.ProcessXliffFile(fileDetails, tmDetails, _translationMemoryService);
				}
				else
				{
					var importSummary = _translationMemoryService.ImportFile(tmDetails.TranslationMemory, fileDetails.Path);
					SetImportSummary(tmDetails, importSummary);
				}
			});
		}

		private static void SetImportSummary(TmDetails tmDetails, ImportSummary importSummary)
		{
			tmDetails.ImportSummary.AddedTusCount = tmDetails.ImportSummary.AddedTusCount + importSummary.AddedTusCount;
			tmDetails.ImportSummary.ReadTusCount = tmDetails.ImportSummary.ReadTusCount + importSummary.ReadTusCount;
			tmDetails.ImportSummary.ErrorCount = tmDetails.ImportSummary.ErrorCount + importSummary.ErrorCount;
		}

		private void BackupTms()
		{
			foreach (var import in ImportCollection)
			{
				foreach (var tm in import.TmsCollection)
				{
					_translationMemoryService.BackUpTm(tm.Path);
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Model;
using Sdl.Community.StarTransit.Shared.Events;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;

namespace Sdl.Community.StarTransit.ViewModel
{
	public class CreateProjectViewModel : WizardViewModelBase
	{
		private int _currentPageNumber;
		private string _displayName;
		private string _tooltip;
		private bool _isNextEnabled;
		private bool _isPreviousEnabled;
		private bool _isValid;
		private bool _projectFinished;
		private bool _projectIsCreating;
		private readonly IWizardModel _wizardModel;
		private readonly IProjectService _projectService;
		private readonly IEventAggregatorService _eventAggregatorService;
		private readonly IDisposable _xliffCreationProgressEvent;
		private readonly IDisposable _projectCreationProgressEvent;
		private readonly IDisposable _fileProgressEvent;
		private readonly IDisposable _createProjectEvent;
		private ObservableCollection<TmSummaryOptions> _tmSummaryOptions;
		private ObservableCollection<TmSummaryOptions> _tmImportProgress;

		public CreateProjectViewModel(IWizardModel wizardModel, IProjectService projectService,IEventAggregatorService eventAggregatorService, object view) : base(view)
		{
			_currentPageNumber = 3;
			_wizardModel = wizardModel;
			_displayName = PluginResources.Wizard_CreateProj_DisplayName;
			_tooltip = PluginResources.Wizard_PackageDetails_Tooltip;
			_isPreviousEnabled = true;
			_isNextEnabled = false;
			_projectService = projectService;
			_eventAggregatorService = eventAggregatorService;
			_fileProgressEvent=_eventAggregatorService?.Subscribe<TmFilesProgress>(OnTmFileProgressChanged);
			_xliffCreationProgressEvent =_eventAggregatorService?.Subscribe<XliffCreationProgress>(OnXliffCreationProgressChanged);
			_projectCreationProgressEvent= _eventAggregatorService?.Subscribe<ProjectCreationProgress>(OnStudioProjectProgressChanged);
			_createProjectEvent = _eventAggregatorService?.Subscribe<CreateStudioProject>(CreateTradosProject);
			TmSummaryOptions = new ObservableCollection<TmSummaryOptions>();
			TmImportProgress = new ObservableCollection<TmSummaryOptions>();
			PropertyChanged += CreateProjectViewModelChanged;
			CanCancel = true;
		}		

		public string PackageName
		{
			get => _wizardModel?.PackageModel?.Result.Name;
		}

		public string ProjectLocation
		{
			get => _wizardModel?.StudioProjectLocation;
		}

		public string Template
		{
			get => _wizardModel?.SelectedTemplate?.Name;
		}
		public string Customer
		{
			get => _wizardModel?.SelectedCustomer?.Name;
		}
		public string DueDate
		{
			get => _wizardModel?.DueDate?.ToString();
		}

		public bool ImportRefMaterials => _wizardModel.ImportRefMeta;

		public string CreateMessage =>  string.Format(PluginResources.CreateProject_Creating, _wizardModel.PackageModel.Result.Name);

		public ObservableCollection<TmSummaryOptions> TmSummaryOptions
		{
			get => _tmSummaryOptions;
			set
			{
				_tmSummaryOptions = value;
				OnPropertyChanged(nameof(TmSummaryOptions));
			}
		}

		public ObservableCollection<TmSummaryOptions> TmImportProgress
		{
			get => _tmImportProgress;
			set
			{
				_tmImportProgress = value;
				OnPropertyChanged(nameof(TmImportProgress));
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

		public int CurrentPageNumber
		{
			get => _currentPageNumber;
			set
			{
				_currentPageNumber = value;
				OnPropertyChanged(nameof(CurrentPageNumber));
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

		public bool ProjectFinished
		{
			get => _projectFinished;
			set
			{
				_projectFinished = value;
				OnPropertyChanged(nameof(ProjectFinished));
			}
		}

		public bool ProjectIsCreating
		{
			get => _projectIsCreating;
			set
			{
				if (_projectIsCreating == value) return;
				_projectIsCreating = value;
				OnPropertyChanged(nameof(ProjectIsCreating));
			}
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
				message = PluginResources.Wizard_ValidationMessage;
				return false;
			}
			return true;
		}

		private void CreateProjectViewModelChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(CurrentPageChanged)) return;
			if (!IsCurrentPage) return;
			TmSummaryOptions.Clear();
			CanFinish = true;
			foreach (var languagePair in _wizardModel.PackageModel.Result.LanguagePairs)
			{
				languagePair.SelectedTranslationMemoryMetadatas.Clear();

				//Create summary data
				var tmSummary = new TmSummaryOptions
				{
					SourceFlag = languagePair.SourceFlag, 
					TargetFlag = languagePair.TargetFlag,
					TargetLanguage = languagePair.TargetLanguage,
					SelectedOption = new List<string>()
				};
				if (languagePair.NoTm)
				{
					tmSummary.SelectedOption.Add(PluginResources.Tm_CreateWitoutTm);
					tmSummary.CreateWithoutTm = true;
				}

				var selectedTms = languagePair.StarTranslationMemoryMetadatas.Where(t => t.IsChecked).ToList();
				if (!_wizardModel.ImportRefMeta)
				{
					selectedTms = selectedTms.Where(t => !t.IsReferenceMeta).ToList();
				}

				if (languagePair.CreateNewTm)
				{
					foreach (var selectedTm in selectedTms)
					{
						if (!selectedTm.Name.Contains(".sdltm"))
						{
							selectedTm.Name = $"{selectedTm.Name}.sdltm";
						}
						selectedTm.LocalTmCreationPath =
							Path.Combine(_wizardModel.PackageModel.Result.Location, selectedTm.Name);

						var selectedTmOption = $"{PluginResources.Tm_CreateTm}: {selectedTm.Name} {PluginResources.CreateProject_Penalty} {selectedTm.TmPenalty}";
						tmSummary.SelectedOption.Add(selectedTmOption);
					}
				}

				languagePair.SelectedTranslationMemoryMetadatas.AddRange(selectedTms);
				if (languagePair.ChoseExistingTm)
				{
					var option = $"{PluginResources.Tm_BrowseTm}: {languagePair.TmName}.sdltm";
					tmSummary.SelectedOption.Add(option) ;
				}

				TmSummaryOptions.Add(tmSummary);
				TmImportProgress.Add(tmSummary);
			}
		}

		private void OnTmFileProgressChanged(TmFilesProgress fileProgress)
		{
			var processingLanguagePair = GetProcessingLanguagePair(fileProgress.TargetLanguage);
			if (processingLanguagePair == null) return;
			processingLanguagePair.FilesImportProgress = string.Format(PluginResources.CreateProject_ImportXliff,
				fileProgress.ProcessingFileNumber, fileProgress.TotalFilesNumber);
		}

		private void OnXliffCreationProgressChanged(XliffCreationProgress xliffProcess)
		{
			var processingLanguagePair = GetProcessingLanguagePair(xliffProcess.TargetLanguage);
			if (processingLanguagePair == null) return;
			processingLanguagePair.XliffImportProgress = xliffProcess.Progress;
		}

		private void OnStudioProjectProgressChanged(ProjectCreationProgress projectCreationProgress)
		{
			var processingLanguagePair = GetProcessingLanguagePair(projectCreationProgress.TargetLanguage);
			if (processingLanguagePair == null) return;
			processingLanguagePair.ProjectLangPairProgress = projectCreationProgress.Progress;
		}

		private TmSummaryOptions GetProcessingLanguagePair(CultureInfo targetCulture)
		{
			return TmImportProgress.FirstOrDefault(l => l.TargetLanguage.Equals(targetCulture));
		}

		private async void CreateTradosProject(CreateStudioProject createEvent)
		{
			ProjectIsCreating = true;
			CanCancel = false;
			CanFinish = false;
			IsComplete = true; // Disable the button while project is creating

			var createdProject = await _projectService.CreateStudioProject(_wizardModel.PackageModel.Result);
			ProjectFinished = true;

			IsPreviousEnabled = false;

			if (createdProject != null)
			{
				_eventAggregatorService.PublishEvent(new ProjectCreated
				{
					CreatedProject = createdProject
				});
			}
		}

		public override void Dispose()
		{
			_projectCreationProgressEvent?.Dispose();
			_xliffCreationProgressEvent?.Dispose();
			_fileProgressEvent?.Dispose();
			_createProjectEvent?.Dispose();
		}
	}
}

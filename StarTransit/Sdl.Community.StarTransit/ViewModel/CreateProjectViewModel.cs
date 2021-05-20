using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Input;
using Sdl.Community.StarTransit.Command;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Model;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.ViewModel
{
	public class CreateProjectViewModel : WizardViewModelBase
	{
		private int _currentPageNumber;
		private string _displayName;
		private string _tooltip;
		private string _errorMessage;
		private bool _isNextEnabled;
		private bool _isPreviousEnabled;
		private bool _isValid;
		private bool _projectFinished;
		private readonly IWizardModel _wizardModel;
		private readonly IProjectService _projectService;
		private ICommand _createProjectCommand;
		private ObservableCollection<TmSummaryOptions> _tmSummaryOptions;

		public CreateProjectViewModel(IWizardModel wizardModel, IProjectService projectService, object view) : base(view)
		{
			_currentPageNumber = 3;
			_wizardModel = wizardModel;
			_displayName = PluginResources.Wizard_CreateProj_DisplayName;
			_tooltip = PluginResources.Wizard_PackageDetails_Tooltip;
			_isPreviousEnabled = true;
			_isNextEnabled = false;
			_projectService = projectService;
			TmSummaryOptions = new ObservableCollection<TmSummaryOptions>();
			PropertyChanged += CreateProjectViewModelChanged;
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

		public ObservableCollection<TmSummaryOptions> TmSummaryOptions
		{
			get => _tmSummaryOptions;
			set
			{
				_tmSummaryOptions = value;
				OnPropertyChanged(nameof(TmSummaryOptions));
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

		public ICommand CreateProjectCommand =>
			_createProjectCommand ?? (_createProjectCommand = new RelayCommand(CreateTradosProject));

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

		private void CreateProjectViewModelChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(CurrentPageChanged)) return;
			if (!IsCurrentPage) return;
			TmSummaryOptions.Clear();
			foreach (var languagePair in _wizardModel.PackageModel.Result.LanguagePairs)
			{
				//Create summary data
				var tmSummary = new TmSummaryOptions
				{
					SourceFlag = languagePair.SourceFlag, TargetFlag = languagePair.TargetFlag
				};
				if (languagePair.NoTm)
				{
					tmSummary.SelectedOption = PluginResources.Tm_CreateWitoutTm;
				}

				var selectedTms = languagePair.StarTranslationMemoryMetadatas.Where(t => t.IsChecked).ToList();
				if (languagePair.CreateNewTm)
				{
					foreach (var selectedTm in selectedTms)
					{
						selectedTm.Name = $"{selectedTm.Name}.sdltm";
						selectedTm.LocalTmCreationPath = Path.Combine(_wizardModel.PackageModel.Result.Location, selectedTm.Name);
					}

					tmSummary.SelectedOption =
						$"{PluginResources.Tm_CreateTm}: {selectedTms[0].Name} {PluginResources.CreateProject_Penalty} {selectedTms[0].TmPenalty}";
				}
				languagePair.SelectedTranslationMemoryMetadatas.AddRange(selectedTms);
				if (languagePair.ChoseExistingTm)
				{
					tmSummary.SelectedOption = $"{PluginResources.Tm_BrowseTm}: {languagePair.TmName}.sdltm";
				}
				TmSummaryOptions.Add(tmSummary);
			}
		}

		private void CreateTradosProject()
		{
			var proj = _projectService.CreateStudioProject(_wizardModel.PackageModel.Result);
			ProjectFinished = true;
		}
	}
}

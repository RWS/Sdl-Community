using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Sdl.Community.StarTransit.Command;
using Sdl.Community.StarTransit.Interface;
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
		private readonly IWizardModel _wizardModel;
		private readonly IProjectService _projectService;
		private ICommand _createProjectCommand;

		public CreateProjectViewModel(IWizardModel wizardModel, IProjectService projectService, object view) : base(view)
		{
			_currentPageNumber = 3;
			_wizardModel = wizardModel;
			_displayName = PluginResources.Wizard_CreateProj_DisplayName;
			_tooltip = PluginResources.Wizard_PackageDetails_Tooltip;
			_isPreviousEnabled = true;
			_isNextEnabled = false;
			_projectService = projectService;
			PropertyChanged += CreateProjectViewModelChanged;
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
			foreach (var languagePair in _wizardModel.PackageModel.Result.LanguagePairs)
			{
				if (languagePair.CreateNewTm)
				{
					//TMs/MT files selected without any penalties will be imported into only one tm
					var selectedTms = languagePair.StarTranslationMemoryMetadatas.Where(t => t.IsChecked)
						.GroupBy(t => t.TmPenalty).ToList();
					languagePair.GroupedTmsByPenalty.AddRange(selectedTms);
				}

				if (!languagePair.ChoseExistingTm) continue;
				{
					//We'll import all the transit TM files into selected TM
					var tmsForSelectedTm = languagePair.StarTranslationMemoryMetadatas.Where(t =>!t.IsMtFile).GroupBy(t => t.TmPenalty).ToList();
					languagePair.GroupedTmsByPenalty.AddRange(tmsForSelectedTm);
				}
			}
		}
		private void CreateTradosProject()
		{
			var proj = _projectService.CreateStudioProject(_wizardModel.PackageModel.Result);
		}
	}
}

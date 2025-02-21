using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.StarTransit.Command;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Service;
using Sdl.Community.StarTransit.Shared.Events;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.ViewModel
{
	public class PackageDetailsViewModel:WizardViewModelBase
	{
		private int _currentPageNumber;
		private string _displayName;
		private string _tooltip;
		private string _errorMessage;
		private bool _isNextEnabled;
		private bool _isPreviousEnabled;
		private bool _isValid;
		private readonly IWizardModel _wizardModel;
		private readonly IStudioService _studioService;
		private readonly IDialogService _dialogService;
		private ICommand _clearCommand;
		private ICommand _browseCommand;
		private ICommand _clearDueDateCommand;
		private DateTime _displayStartDate;
		private readonly IDisposable _errorMessageEvent;

		public PackageDetailsViewModel(IWizardModel wizardModel, IPackageService packageService,
			IDialogService folderService, IStudioService studioService,string projectsXmlFilePath,IEventAggregatorService eventAggregator, object view) : base(view)
		{
			_wizardModel = wizardModel;
			CurrentPageNumber = 1;
			_displayName = PluginResources.Wizard_PackageDetails_DisplayName;
			_tooltip = PluginResources.Wizard_PackageDetails_Tooltip;
			IsPreviousEnabled = false;
			IsNextEnabled = true;
			CanCancel = true;
			_isValid = false;
			_dialogService = folderService;
			_studioService = studioService;
			PackageModel = new AsyncTaskWatcherService<PackageModel>(
				packageService.OpenPackage(_wizardModel.TransitFilePathLocation, _wizardModel.PathToTempFolder));
			Customers = new AsyncTaskWatcherService<List<Customer>>(_studioService.GetCustomers(projectsXmlFilePath));
			ProjectTemplates = new ObservableCollection<ProjectTemplateInfo>(_studioService.GetProjectTemplates());
			SelectedProjectTemplate = ProjectTemplates[0];
			DueDate = null;
			_displayStartDate = DateTime.Now;
			_errorMessage = string.Empty;
			PropertyChanged += PackageDetailsViewModelChanged;

			_errorMessageEvent = eventAggregator.Subscribe<Error>(OnErrorOccured);
		}

		private void PackageDetailsViewModelChanged(object sender, PropertyChangedEventArgs e)
		{
			if (PackageModel.Result == null) return;
			PackageModel.Result.ProjectTemplate = SelectedProjectTemplate;
			PackageModel.Result.Customer = SelectedCustomer;
			PackageModel.Result.DueDate = DueDate;
			PackageModel.Result.Location = StudioProjectLocation;
		}

		public AsyncTaskWatcherService<PackageModel> PackageModel
		{
			get => _wizardModel.PackageModel;
			set
			{
				_wizardModel.PackageModel = value;
				OnPropertyChanged(nameof(PackageModel));
			}
		}

		public string StudioProjectLocation
		{
			get => _wizardModel.StudioProjectLocation;
			set
			{
				_wizardModel.StudioProjectLocation = value;
				ValidateLocation(value);
				OnPropertyChanged(nameof(StudioProjectLocation));
			}
		}

		public AsyncTaskWatcherService<List<Customer>> Customers
		{
			get => _wizardModel.Customers;
			set
			{
				_wizardModel.Customers = value;
				OnPropertyChanged(nameof(Customers));
			}
		}

		public Customer SelectedCustomer
		{
			get => _wizardModel.SelectedCustomer;
			set
			{
				_wizardModel.SelectedCustomer = value;
				OnPropertyChanged(nameof(SelectedCustomer));
			}
		}

		public ObservableCollection<ProjectTemplateInfo> ProjectTemplates
		{
			get => _wizardModel.ProjectTemplates;
			set
			{
				_wizardModel.ProjectTemplates = value;
				OnPropertyChanged(nameof(ProjectTemplates));
			}
		}

		public ProjectTemplateInfo SelectedProjectTemplate
		{
			get => _wizardModel.SelectedTemplate;
			set
			{
				_wizardModel.SelectedTemplate = value;
				ReadProjectTemplateInfo(value);
				OnPropertyChanged(nameof(SelectedProjectTemplate));
			}
		}

		public DateTime? DueDate
		{
			get => _wizardModel.DueDate;
			set
			{
				_wizardModel.DueDate = value;
				OnPropertyChanged(nameof(DueDate));
			}
		}

		public DateTime DisplayStartDate
		{
			get => _displayStartDate;
			set
			{
				_displayStartDate = value;
				OnPropertyChanged(nameof(DisplayStartDate));
			}
		}

		public string ErrorMessage
		{
			get => _errorMessage;
			set
			{
				if (_errorMessage == value) return;
				_errorMessage = value;
				OnPropertyChanged(nameof(ErrorMessage));
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

		public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new RelayCommand(ClearLocation));
		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new CommandHandler(Browse));

		public ICommand ClearDueDateCommand =>
			_clearDueDateCommand ?? (_clearDueDateCommand = new RelayCommand(ClearDate));

		private void ClearDate()
		{
			DueDate = null;
		}

		private void Browse(object commandParameter)
		{
			ErrorMessage = string.Empty;
			if(commandParameter != null)
			{
				var parameter = commandParameter.ToString();
				if (parameter.Equals(PluginResources.BrowseLocation))
				{
					var location = _dialogService.ShowFolderDialog(PluginResources.PackageDetails_FolderLocation);
					if (string.IsNullOrEmpty(location)) return;

					StudioProjectLocation = location;
					PackageModel.Result.Location = location;
				}
				if (parameter.Equals(PluginResources.SelectTemplate))
				{
					var selectedTemplate = _dialogService.ShowFileDialog("Project Templates (*.sdltpl)| *.sdltpl",PluginResources.TemplateTitle);
					if (!string.IsNullOrEmpty(selectedTemplate))
					{
						var template = new ProjectTemplateInfo
						{
							Uri = new Uri(selectedTemplate),
							Name = Path.GetFileNameWithoutExtension(selectedTemplate),
							Id = Guid.NewGuid()
						};

						ProjectTemplates.Add(template);
						SelectedProjectTemplate = template;
					}
				}
			}
			
		}

		private void ValidateLocation(string location)
		{
			ErrorMessage = string.Empty;

			if (string.IsNullOrEmpty(location))
			{
				ErrorMessage = PluginResources.Wizard_ValidationMessage;
				IsValid = false;
				return;
			}

			if (!Path.IsPathRooted(location))
			{
				IsValid = false;
				ErrorMessage = PluginResources.Details_LocationValidation;
			}
			else if (Directory.Exists(location))
			{
				var isEmptyFolder = !Directory
					.GetFiles(location, "*.*", SearchOption.AllDirectories)
					.Any();
				if (!isEmptyFolder)
				{
					ErrorMessage = PluginResources.EmptyFolder_Error;
					IsValid = false;
				}
				else
				{
					IsValid = true;
				}
			}
			else
			{
				IsValid = true;
			}

			var traversalRequest = new TraversalRequest(FocusNavigationDirection.Next);
			var focused = (UIElement)Keyboard.FocusedElement;
			focused?.MoveFocus(traversalRequest);
		}

		private void ClearLocation()
		{
			StudioProjectLocation = string.Empty;
		}

		private async System.Threading.Tasks.Task ReadProjectTemplateInfo(ProjectTemplateInfo selectedProjectTemplate)
		{
			if (PackageModel.Result == null) return;

			var templatePackageModel = await  _studioService.GetModelBasedOnStudioTemplate(selectedProjectTemplate.Uri.LocalPath,
				PackageModel.Result.SourceLanguage, PackageModel.Result.TargetLanguages);

			UpdateUiBasedOnTemplate(templatePackageModel);
		}

		private void UpdateUiBasedOnTemplate(PackageModel templatePackageModel)
		{
			if (templatePackageModel is null)
			{
				ClearUi();
			}
			else
			{
				StudioProjectLocation = Path.Combine(templatePackageModel.Location, PackageModel.Result.Name);
				if (templatePackageModel.Customer != null)
				{
					var selectedCustomer =
						Customers.Result.FirstOrDefault(c => c.Name != null && c.Name.Equals(templatePackageModel.Customer.Name));
					if (selectedCustomer != null)
					{
						SelectedCustomer = selectedCustomer;
					}
				}

				if (templatePackageModel.DueDate != null)
				{
					DueDate = templatePackageModel.DueDate;
				}

				if (templatePackageModel.LanguagePairs == null) return;

				foreach (var languagePairOption in templatePackageModel.LanguagePairs)
				{
					var selectedLp = PackageModel.Result.LanguagePairs.FirstOrDefault(t =>
						t.TargetLanguage.Name.Equals(languagePairOption.TargetLanguage.Name));
					if (selectedLp == null) continue;
					if (languagePairOption.CreateNewTm)
					{
						selectedLp.CreateNewTm = true;
						selectedLp.NoTm = false;
						SetTmMetadatas(selectedLp, languagePairOption.TemplatePenalty);
					}

					if (!languagePairOption.ChoseExistingTm) continue;
					selectedLp.TmName = languagePairOption.TmName;
					selectedLp.TmPath = languagePairOption.TmPath;
					SetTmMetadatas(selectedLp, 0);
					selectedLp.ChoseExistingTm = true;
					selectedLp.NoTm = false;
					selectedLp.CreateNewTm = false;
				}
			}
		}

		private static void SetTmMetadatas(LanguagePair selectedLp, int penalty)
		{
			foreach (var tmMetadata in selectedLp.StarTranslationMemoryMetadatas)
			{
				tmMetadata.TmPenalty = penalty;
				tmMetadata.IsChecked = true;
				if (!string.IsNullOrEmpty(selectedLp.TmPath))
				{
					tmMetadata.LocalTmCreationPath = selectedLp.TmPath;
				}
			}
		}

		private void ClearUi()
		{
			StudioProjectLocation = string.Empty;
			SelectedCustomer = Customers.Result[0];
			DueDate = null;
			foreach (var languagePair in PackageModel.Result.LanguagePairs)
			{
				languagePair.NoTm = true;
				languagePair.ChoseExistingTm = false;
				languagePair.CreateNewTm = false;
				foreach (var tmMetadata in languagePair.StarTranslationMemoryMetadatas)
				{
					tmMetadata.TmPenalty = 0;
					tmMetadata.IsChecked = false;
					tmMetadata.LocalTmCreationPath = string.Empty;
				}
				languagePair.TmPath = string.Empty;
				languagePair.TmName = string.Empty;
			}
		}
		private void OnErrorOccured(Error error)
		{
			ErrorMessage = error.ErrorMessage;
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

		public override void Dispose()
		{
			_errorMessageEvent?.Dispose();
		}
	}
}

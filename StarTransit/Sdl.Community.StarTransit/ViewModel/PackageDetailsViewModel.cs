using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StarTransit.Command;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Model;
using Sdl.Community.StarTransit.Service;
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
		private bool _isNextEnabled;
		private bool _isPreviousEnabled;
		private bool _isValid;
		private readonly IWizardModel _wizardModel;
		private readonly IFolderDialogService _dialogService;
		private readonly IStudioService _studioService;
		private ICommand _clearCommand;
		private ICommand _browseCommand;
		private ICommand _clearDueDateCommand;
		private DateTime _displayStartDate;

		public PackageDetailsViewModel(IWizardModel wizardModel, IPackageService packageService,
			IFolderDialogService folderService, IStudioService studioService, object view) : base(view)
		{
			_wizardModel = wizardModel;
			_currentPageNumber = 1;
			_displayName = PluginResources.Wizard_PackageDetails_DisplayName;
			IsPreviousEnabled = false;
			_isNextEnabled = true;
			_isValid = true; //TODO remove this
			_dialogService = folderService;
			_studioService = studioService;
			PackageModel = new AsyncTaskWatcherService<PackageModel>(
				packageService.OpenPackage(_wizardModel.TransitFilePathLocation, _wizardModel.PathToTempFolder));
			Customers = new AsyncTaskWatcherService<List<Customer>>(_studioService.GetCustomers());
			ProjectTemplates = new List<ProjectTemplateInfo>(_studioService.GetProjectTemplates());
			SelectedProjectTemplate = ProjectTemplates[0];
			DueDate = null;
			_displayStartDate = DateTime.Now;
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

		public List<ProjectTemplateInfo> ProjectTemplates
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
		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new RelayCommand(BrowseLocation));

		public ICommand ClearDueDateCommand =>
			_clearDueDateCommand ?? (_clearDueDateCommand = new RelayCommand(ClearDate));

		private void ClearDate()
		{
			DueDate = null;
		}

		private void BrowseLocation()
		{
			var location = _dialogService.ShowDialog(PluginResources.PackageDetails_FolderLocation);
			if(string.IsNullOrEmpty(location))return;
			StudioProjectLocation = location;
		}

		private void ClearLocation()
		{
			StudioProjectLocation = string.Empty;
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
	}
}

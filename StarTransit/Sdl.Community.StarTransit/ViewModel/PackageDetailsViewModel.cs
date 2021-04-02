using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Model;
using Sdl.Community.StarTransit.Service;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;

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

		public PackageDetailsViewModel(IWizardModel wizardModel,IPackageService packageService,object view) : base(view)
		{
			_wizardModel = wizardModel;
			_currentPageNumber = 1;
			_displayName = PluginResources.Wizard_PackageDetails_DisplayName;
			_isPreviousEnabled = false;
			_isNextEnabled = true;
			_isValid = true; //TODO remove this
			PackageModel = new AsyncTaskWatcherService<PackageModel>(
				packageService.OpenPackage(_wizardModel.TransitFilePathLocation, _wizardModel.PathToTempFolder));
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

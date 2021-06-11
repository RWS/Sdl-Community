using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NLog.LayoutRenderers;
using Sdl.Community.StarTransit.Command;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Model;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;

namespace Sdl.Community.StarTransit.ViewModel
{
	public class ReturnPackageWindowViewModel : BaseModel,IDisposable
	{
		private readonly IDialogService _dialogService;
		private ICommand _browseCommand;
		private ICommand _clearCommand;
		private string _returnPackageLocation;
		private string _errorMessage;
		private bool _isCreateButtonEnabled;

		public ReturnPackageWindowViewModel(IReturnPackage returnPackage, IDialogService dialogService)
		{
			_dialogService = dialogService;
			ReturnPackage = returnPackage;

			foreach (var returnFile in ReturnPackage.ReturnFilesDetails)
			{
				returnFile.PropertyChanged += ReturnFile_PropertyChanged;
			}
		}

		public string ReturnPackageLocation
		{
			get => _returnPackageLocation;
			set
			{
				if (_returnPackageLocation == value) return;
				_returnPackageLocation = value;

				if (!string.IsNullOrEmpty(_returnPackageLocation))
				{
					ValidateLocation();
				}
				else
				{
					ErrorMessage = string.Empty;
				}

				OnPropertyChanged(nameof(ReturnPackageLocation));
			}
		}

		private void ValidateLocation()
		{
			ErrorMessage = string.Empty;
			if (!Path.IsPathRooted(ReturnPackageLocation))
			{
				ErrorMessage = PluginResources.Details_LocationValidation;
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

		public bool IsCreateButtonEnabled
		{
			get => _isCreateButtonEnabled;
			set
			{
				if (_isCreateButtonEnabled == value) return;
				_isCreateButtonEnabled = value;
				OnPropertyChanged(nameof(IsCreateButtonEnabled));
			}
		}

		public IReturnPackage ReturnPackage { get; set; }

		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new RelayCommand(BrowseLocation));
		public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new RelayCommand(ClearLocation));

		private void ClearLocation()
		{
			ErrorMessage = string.Empty;
			ReturnPackageLocation = string.Empty;
		}

		private void BrowseLocation()
		{
			var location = _dialogService.ShowFolderDialog(PluginResources.PackageDetails_FolderLocation);
			if (string.IsNullOrEmpty(location)) return;

			ReturnPackageLocation = location;
		}

		private void ReturnFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(ReturnFileDetails.IsChecked)) return;
			var anyFilesSelected = ReturnPackage.ReturnFilesDetails.Any(f => f.IsChecked);
			if (string.IsNullOrEmpty(ErrorMessage) && anyFilesSelected)
			{
				IsCreateButtonEnabled = true;
			}
			else
			{
				IsCreateButtonEnabled = false;
			}
		}

		public void Dispose()
		{
			foreach (var returnFile in ReturnPackage.ReturnFilesDetails)
			{
				returnFile.PropertyChanged -= ReturnFile_PropertyChanged;
			}
		}
	}
}

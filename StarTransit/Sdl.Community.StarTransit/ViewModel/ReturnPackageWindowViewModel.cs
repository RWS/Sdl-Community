using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Sdl.Community.StarTransit.Command;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Model;
using Sdl.Community.StarTransit.Shared.Events;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.ProjectAutomation.Core;
using Task = System.Threading.Tasks.Task;

namespace Sdl.Community.StarTransit.ViewModel
{
	public class ReturnPackageWindowViewModel : BaseModel,IDisposable
	{
		private readonly IDialogService _dialogService;
		private readonly IReturnPackageService _returnPackageService;
		private readonly IEventAggregatorService _eventAggregatorService;
		private ICommand _browseCommand;
		private ICommand _createPackage;
		private string _returnPackageLocation;
		private string _errorMessage;
		private bool _isCreateButtonEnabled;
		private bool? _selectAll;
		public List<EncodingInfo> Encodings { get; set; }
		private EncodingInfo _selectedFileNameEncoding;

		public ReturnPackageWindowViewModel(IReturnPackage returnPackage,IReturnPackageService returnPackageService, IDialogService dialogService,IEventAggregatorService eventAggregatorService)
		{
			_dialogService = dialogService;
			_returnPackageService = returnPackageService;
			_eventAggregatorService = eventAggregatorService;
			ReturnPackage = returnPackage;
			SelectAll = false;
			foreach (var returnFile in ReturnPackage.ReturnFilesDetails)
			{
				returnFile.PropertyChanged += ReturnFile_PropertyChanged;
			}

			LoadEncodingsOptions();
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

		public bool? SelectAll
		{
			get => _selectAll;
			set
			{
				if (_selectAll == value) return;
				_selectAll = value;
				CheckAllFiles(value);
				OnPropertyChanged(nameof(SelectAll));
			}
		}

		public EncodingInfo SelectedFileNameEncoding
		{
			get => _selectedFileNameEncoding;
			set
			{
				_selectedFileNameEncoding = value;
				OnPropertyChanged(nameof(SelectedFileNameEncoding));
			}
		}

		private void CheckAllFiles(bool? value)
		{
			if (value == null) return;
			foreach (var file in ReturnPackage.ReturnFilesDetails)
			{
				file.IsChecked = (bool)value;
			}
		}

		public IReturnPackage ReturnPackage { get; set; }

		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new RelayCommand(BrowseLocation));

		public ICommand CreatePackageCommand =>
			_createPackage ?? (_createPackage = new AwaitableCommand(CreatePackage));

		private async Task CreatePackage()
		{
			ReturnPackage.FolderLocation = !string.IsNullOrEmpty(ReturnPackageLocation) ? ReturnPackageLocation : ReturnPackage.ProjectLocation;
			var selectedFiles = ReturnPackage.ReturnFilesDetails.Where(f => f.IsChecked);
			var targetFiles = new List<ProjectFile>();
			foreach (var selectedFile in selectedFiles)
			{
				var targetFile = ReturnPackage.TargetFiles.FirstOrDefault(f => f.Id.Equals(selectedFile.Id));
				if (targetFile != null)
				{
					targetFiles.Add(targetFile);
				}
			}
			ReturnPackage.SelectedTargetFilesForImport.AddRange(targetFiles);
			var exportedWithSuccess =await Task.Run(() => _returnPackageService.ExportFiles(ReturnPackage,SelectedFileNameEncoding.CodePage));

			if (exportedWithSuccess)
			{
				_eventAggregatorService?.PublishEvent(new OpenReturnPackageLocation { RetuntPackageLocation = ReturnPackage.FolderLocation });
			}
			else
			{
				ErrorMessage = "Return package could not be created. Please see log file for more details.";
			}
		}
		private void LoadEncodingsOptions()
		{
			Encodings = Encoding.GetEncodings().ToList();
			var westernEncoding = Encodings.FirstOrDefault(e => e.CodePage.Equals(850));
			if (westernEncoding != null)
			{
				SelectedFileNameEncoding = westernEncoding;
			}
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
		
			if (string.IsNullOrEmpty(ErrorMessage) && AnyFileSelected())
			{
				IsCreateButtonEnabled = true;
			}
			else
			{
				IsCreateButtonEnabled = false;
			}

			if (AreAllFilesSelected())
			{
				SelectAll = true;
			}
			else
			{
				var allUnchecked = ReturnPackage.ReturnFilesDetails.All(f => !f.IsChecked);
				SelectAll = allUnchecked ? (bool?)false : null;
			}
		}

		private bool AreAllFilesSelected()
		{
			return ReturnPackage.ReturnFilesDetails.All(f => f.IsChecked);
		}

		private bool AnyFileSelected()
		{
			return ReturnPackage.ReturnFilesDetails.Any(f => f.IsChecked);
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

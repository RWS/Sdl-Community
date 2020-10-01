using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Sdl.Community.SdlFreshstart.Commands;
using Sdl.Community.SdlFreshstart.Helpers;
using Sdl.Community.SdlFreshstart.Model;
using Sdl.Community.SdlFreshstart.Properties;
using Sdl.Community.SdlFreshstart.Services;

namespace Sdl.Community.SdlFreshstart.ViewModel
{
	public class MultiTermViewModel : BaseModel
	{
		private readonly MainWindow _mainWindow;
		private readonly IMessageService _messageService;
		private readonly VersionService _versionService;
		private readonly RegistryHelper _registryHelper;
		private readonly Persistence _persistence;
		private readonly string _userName;
		private bool _checkAll;
		private string _folderDescription;
		private bool _isRemoveEnabled;
		private bool _isRepairEnabled;
		private bool _isRestoreEnabled;
		private List<MultiTermLocationListItem> _multiTermLocationCollection;
		private List<MultitermVersion> _multiTermVersionsCollection;
		private string _packageCache = @"C:\ProgramData\Package Cache\SDL";
		private string _removeBtnColor;
		private ICommand _removeCommand;
		private string _removeForeground;
		private string _repairBtnColor;
		private ICommand _repairCommand;
		private string _repairForeground;
		private ICommand _restoreCommand;
		private MultiTermLocationListItem _selectedLocation;

		public MultiTermViewModel(MainWindow mainWindow, IMessageService messageService, VersionService versionService, RegistryHelper registryHelper)
		{
			_mainWindow = mainWindow;
			_messageService = messageService;
			_versionService = versionService;
			_registryHelper = registryHelper;
			_userName = Environment.UserName;
			_persistence = new Persistence();
			_folderDescription = string.Empty;
			_isRemoveEnabled = false;
			_isRepairEnabled = false;
			_checkAll = false;
			_removeBtnColor = "LightGray";
			_removeForeground = "Gray";
			_repairBtnColor = "LightGray";
			_repairForeground = "Gray";
			FillMultiTermVersionList();
			FillMultiTermLocationList();
		}

		public bool CheckAll
		{
			get => _checkAll;

			set
			{
				if (Equals(value, _checkAll))
				{
					return;
				}
				_checkAll = value;
				OnPropertyChanged(nameof(CheckAll));
				CheckAllLocations(value);
			}
		}

		public bool IsRemoveEnabled
		{
			get => _isRemoveEnabled;

			set
			{
				if (Equals(value, _isRemoveEnabled))
				{
					return;
				}
				_isRemoveEnabled = value;
				OnPropertyChanged(nameof(IsRemoveEnabled));
			}
		}

		public bool IsRepairEnabled
		{
			get => _isRepairEnabled;

			set
			{
				if (Equals(value, _isRepairEnabled))
				{
					return;
				}
				_isRepairEnabled = value;
				OnPropertyChanged(nameof(IsRepairEnabled));
			}
		}

		public bool IsRestoreEnabled
		{
			get => _isRestoreEnabled;

			set
			{
				if (Equals(value, _isRestoreEnabled))
				{
					return;
				}
				_isRestoreEnabled = value;
				OnPropertyChanged(nameof(IsRestoreEnabled));
			}
		}

		public List<MultiTermLocationListItem> MultiTermLocationCollection
		{
			get => _multiTermLocationCollection;
			set
			{
				if (Equals(value, _multiTermLocationCollection))
				{
					return;
				}
				_multiTermLocationCollection = value;
				OnPropertyChanged(nameof(MultiTermLocationCollection));
			}
		}

		public List<MultitermVersion> MultiTermVersionsCollection
		{
			get => _multiTermVersionsCollection;
			set
			{
				if (Equals(value, _multiTermVersionsCollection))
				{
					return;
				}
				_multiTermVersionsCollection = value;
				OnPropertyChanged(nameof(MultiTermVersionsCollection));
			}
		}

		public string RemoveBtnColor
		{
			get => _removeBtnColor;

			set
			{
				if (Equals(value, _removeBtnColor))
				{
					return;
				}
				_removeBtnColor = value;
				OnPropertyChanged(nameof(RemoveBtnColor));
			}
		}

		public ICommand RemoveCommand => _removeCommand ??= new CommandHandler(RemoveFromLocations, true);

		public string RemoveForeground
		{
			get => _removeForeground;

			set
			{
				if (Equals(value, _removeForeground))
				{
					return;
				}
				_removeForeground = value;
				OnPropertyChanged(nameof(RemoveForeground));
			}
		}

		public string RepairBtnColor
		{
			get => _repairBtnColor;

			set
			{
				if (Equals(value, _repairBtnColor))
				{
					return;
				}
				_repairBtnColor = value;
				OnPropertyChanged(nameof(RepairBtnColor));
			}
		}

		public ICommand RepairCommand => _repairCommand ??= new CommandHandler(RepairMultiTerm, true);

		public string RepairForeground
		{
			get => _repairForeground;

			set
			{
				if (Equals(value, _repairForeground))
				{
					return;
				}
				_repairForeground = value;
				OnPropertyChanged(nameof(RepairForeground));
			}
		}

		public ICommand RestoreCommand => _restoreCommand ??= new CommandHandler(RestoreLocations, true);

		public MultiTermLocationListItem SelectedLocation
		{
			get => _selectedLocation;
			set
			{
				_selectedLocation = value;
				OnPropertyChanged();
			}
		}

		private bool AnyLocationSelected()
		{
			return MultiTermLocationCollection.Any(l => l.IsSelected);
		}

		private bool AnyVersionSelected()
		{
			return MultiTermVersionsCollection.Any(v => v.IsSelected);
		}

		private void CheckAllLocations(bool check)
		{
			foreach (var location in MultiTermLocationCollection)
			{
				location.IsSelected = check;
			}
		}

		private MultitermVersion LatestMultitermVersion => MultiTermVersionsCollection.FirstOrDefault();

		private void FillMultiTermLocationList()
		{
			var listOfProperties = new List<string>
			{
				nameof(MultitermVersion.MultiTermRoaming),
				nameof(MultitermVersion.MultiTermLocal),
				nameof(MultitermVersion.MultiTermProgramDataSettings),
				nameof(MultitermVersion.MultiTermProgramDataUpdates),
				nameof(MultitermVersion.MultiTermRegistryKey)
			};

			_multiTermLocationCollection = new List<MultiTermLocationListItem>();
			foreach (var property in listOfProperties)
			{
				_multiTermLocationCollection.Add(new MultiTermLocationListItem
				{
					DisplayName = (string)LatestMultitermVersion?.GetType().GetProperty(property)?.GetValue(LatestMultitermVersion),
					Description = (string)typeof(LocationsDescription).GetProperty(property)?.GetValue(null, null),
					IsSelected = true,
					Alias = property
				});
			}

			foreach (var multiTermLocation in _multiTermLocationCollection)
			{
				multiTermLocation.PropertyChanged += MultiTermLocation_PropertyChanged;
			}
		}

		private void FillMultiTermVersionList()
		{
			_multiTermVersionsCollection = new List<MultitermVersion>(_versionService.GetInstalledMultitermVersions());
			foreach (var multiTermVersion in _multiTermVersionsCollection)
			{
				multiTermVersion.PropertyChanged += MultiTermVersion_PropertyChanged;
			}
		}

		private string GetMsiName(MultitermVersion selectedVersion)
		{
			var msiName = $"MTCore{selectedVersion.MajorVersion}.msi";
			return msiName;
		}

		private List<LocationDetails> GetLocationsForSelectedVersions()
		{
			var allFolders = _persistence.Load(false);
			var selectedVersions = MultiTermVersionsCollection.Where(s => s.IsSelected).ToList();
			var locationsForSelectedVersion = new List<LocationDetails>();
			if (selectedVersions.Any())
			{
				foreach (var version in selectedVersions)
				{
					var locations = allFolders.Where(v => v.Version.Equals(version.VersionName)).ToList();
					locationsForSelectedVersion.AddRange(locations);
				}
				return locationsForSelectedVersion;
			}
			return allFolders;
		}

		private bool MultiTermIsRunning()
		{
			var processList = Process.GetProcesses();
			var multiTermProcesses = processList.Where(p => p.ProcessName.Contains("MultiTerm")).ToList();
			return multiTermProcesses.Any();
		}

		private void MultiTermLocation_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SetRemoveBtnColors();
		}

		private void MultiTermVersion_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SetRemoveBtnColors();
		}

		private async void RemoveFromLocations()
		{
			var result = _messageService.ShowConfirmationMessage(Constants.Confirmation, Constants.RemoveMessage);
			if (result == MessageBoxResult.Yes)
			{
				if (!MultiTermIsRunning())
				{
					var controller = await ShowProgress(Constants.Wait, Constants.RemoveFilesMessage);

					var foldersToClearOrRestore = new List<LocationDetails>();
					var registryToClearOrRestore = new List<LocationDetails>();

					var selectedMultiTermVersions = MultiTermVersionsCollection.Where(s => s.IsSelected).ToList();
					var selectedMultiTermLocations = MultiTermLocationCollection.Where(f => f.IsSelected).ToList();
					var locations = new List<LocationDetails>();
					if (selectedMultiTermVersions.Any())
					{
						locations = Paths.GetMultiTermLocationsFromVersions(selectedMultiTermLocations.Select(l => l.Alias).ToList(),
							selectedMultiTermVersions);

						var registryLocations = locations.TakeWhile(l => l.Alias == nameof(MultitermVersion.MultiTermRegistryKey)).ToList();
						registryToClearOrRestore.AddRange(registryLocations);

						var folderLocations = locations.TakeWhile(l => l.Alias != nameof(MultitermVersion.MultiTermRegistryKey)).ToList();
						foldersToClearOrRestore.AddRange(folderLocations);
					}

					//save settings
					_persistence.SaveSettings(locations, false);
					await FileManager.BackupFiles(foldersToClearOrRestore);
					await _registryHelper.BackupKeys(registryToClearOrRestore);

					RemoveFromFolders(foldersToClearOrRestore);
					RemoveFromRegistry(registryToClearOrRestore);

					//to close the message
					await controller.CloseAsync();
				}
				else
				{
					_messageService.ShowWarningMessage(Constants.MultitermRun, Constants.RemoveFoldersMessage);
				}
			}
		}

		private void RemoveFromRegistry(List<LocationDetails> registryToClearOrRestore)
		{
			try
			{
				_registryHelper.DeleteKeys(registryToClearOrRestore, false);
			}
			catch (Exception ex)
			{
				_messageService.ShowWarningMessage(Constants.Warning, string.Format(Constants.RegistryNotDeleted, ex.Message));
			}
		}

		private void RemoveFromFolders(List<LocationDetails> foldersToClearOrRestore)
		{
			try
			{
				FileManager.RemoveFromSelectedFolderLocations(foldersToClearOrRestore);
			}
			catch
			{
				_messageService.ShowWarningMessage(Constants.Warning, Constants.FilesNotDeletedMessage);
			}
		}

		private async Task<ProgressDialogController> ShowProgress(string title, string message)
		{
			var controller = await _mainWindow.ShowProgressAsync(title, message);
			controller.SetIndeterminate();
			return controller;
		}

		private void RepairMultiTerm()
		{
			if (!MultiTermIsRunning())
			{
				if (Directory.Exists(_packageCache))
				{
					var selectedVersions = MultiTermVersionsCollection.Where(s => s.IsSelected).ToList();
					foreach (var selectedVersion in selectedVersions)
					{
						RunRepair(selectedVersion);
					}
				}
			}
			else
			{
				_messageService.ShowWarningMessage(Constants.MultitermRun, Constants.MultitermRepairMessage);
			}
		}

		private async Task RestoreFolders(List<LocationDetails> locationsToRestore)
		{
			var foldersToRestore = locationsToRestore
				.TakeWhile(l => l.Alias != nameof(MultitermVersion.MultiTermRegistryKey)).ToList();
			await FileManager.RestoreBackupFiles(foldersToRestore);
		}

		private async Task RestoreRegistry(List<LocationDetails> locationsToRestore)
		{
			var registryToRestore = locationsToRestore
				.TakeWhile(l => l.Alias == nameof(MultitermVersion.MultiTermRegistryKey)).ToList();
			try
			{
				await _registryHelper.RestoreKeys(registryToRestore);
			}
			catch (Exception e)
			{
				_messageService.ShowWarningMessage(Constants.Warning,
					string.Format(Resources.NotAllRegistriesCouldBeRestored, e.Message));
			}
		}

		private async void RestoreLocations()
		{
			var result = _messageService.ShowConfirmationMessage(Constants.Confirmation, Constants.RestoreMessage);

			if (result != MessageBoxResult.Yes) return;
			if (!MultiTermIsRunning())
			{
				var controller = await ShowProgress(Constants.Wait, Constants.RestoringMessage);

				var locationsToRestore = GetLocationsForSelectedVersions();

				await RestoreFolders(locationsToRestore);
				await RestoreRegistry(locationsToRestore);

				UnselectGrids();

				await controller.CloseAsync();
			}
			else
			{
				_messageService.ShowWarningMessage(Constants.MultitermRun, Constants.MultitermCloseMessage);
			}
		}

		private void RunRepair(MultitermVersion selectedVersion)
		{
			var directoriesPath = new DirectoryInfo(_packageCache)
				.GetDirectories()
				.Where(n => n.Name.Contains(selectedVersion.CacheFolderName))
				.Select(n => n.FullName).ToList();

			foreach (var directoryPath in directoriesPath)
			{
				var msiName = GetMsiName(selectedVersion);
				var moduleDirectoryPath = Path.Combine(directoryPath, "modules");
				if (Directory.Exists(moduleDirectoryPath))
				{
					var msiFile = Path.Combine(moduleDirectoryPath, msiName);
					if (File.Exists(msiFile))
					{
						var process = new ProcessStartInfo
						{
							FileName = "msiexec",
							WorkingDirectory = moduleDirectoryPath,
							Arguments = "/fa " + msiName,
							Verb = "runas"
						};
						Process.Start(process);
					}
				}
			}
		}

		private void SetRemoveBtnColors()
		{
			if (AnyLocationSelected() && AnyVersionSelected())
			{
				IsRemoveEnabled = true;
				RemoveBtnColor = "#3D9DAA";
				RemoveForeground = "WhiteSmoke";
			}
			else
			{
				IsRemoveEnabled = false;
				RemoveBtnColor = "LightGray";
				RemoveForeground = "Gray";
			}

			if (AnyVersionSelected())
			{
				IsRepairEnabled = true;
				RepairBtnColor = "#3D9DAA";
				RepairForeground = "WhiteSmoke";
			}
			else
			{
				IsRepairEnabled = false;
				RepairBtnColor = "LightGray";
				RepairForeground = "Gray";
			}
		}

		private void UnselectGrids()
		{
			var selectedVersions = MultiTermVersionsCollection.Where(v => v.IsSelected).ToList();
			foreach (var version in selectedVersions)
			{
				version.IsSelected = false;
			}

			var selectedLocations = MultiTermLocationCollection.Where(l => l.IsSelected).ToList();
			foreach (var selectedLocation in selectedLocations)
			{
				selectedLocation.IsSelected = false;
			}
			CheckAll = false;
		}
	}
}
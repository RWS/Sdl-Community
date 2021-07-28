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
using NLog;
using Sdl.Community.SdlFreshstart.Commands;
using Sdl.Community.SdlFreshstart.Helpers;
using Sdl.Community.SdlFreshstart.Model;
using Sdl.Community.SdlFreshstart.Properties;
using Sdl.Community.SdlFreshstart.Services;

namespace Sdl.Community.SdlFreshstart.ViewModel
{
	public class StudioViewModel : BaseModel
	{
		private readonly MainWindow _mainWindow;
		private readonly IMessageService _messageService;
		private readonly IRegistryHelper _registryHelper;
		private readonly string _packageCache = @"C:\ProgramData\Package Cache\SDL";
		private readonly Persistence _persistenceSettings;
		private readonly VersionService _versionService;
		private bool _checkAll;
		private ObservableCollection<StudioLocationListItem> _locations;
		private bool _isRemoveEnabled;
		private bool _isRepairEnabled;
		private string _removeBtnColor;
		private ICommand _removeCommand;
		private string _removeForeground;
		private string _repairBtnColor;
		private ICommand _repairCommand;
		private string _repairForeground;
		private string _restoreBtnColor;
		private ICommand _restoreCommand;
		private string _restoreForeground;
		private StudioLocationListItem _selectedLocation;
		private ObservableCollection<StudioVersion> _studioVersionsCollection;
		private bool _registryKeyChecked;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public StudioViewModel(MainWindow mainWindow, VersionService versionService, IMessageService messageService, IRegistryHelper registryHelper)
		{
			_versionService = versionService;
			_messageService = messageService;
			_registryHelper = registryHelper;
			_mainWindow = mainWindow;
			_persistenceSettings = new Persistence();
			_isRemoveEnabled = false;
			_isRepairEnabled = false;
			_checkAll = false;
			_removeBtnColor = "LightGray";
			_removeForeground = "Gray";
			_repairBtnColor = "LightGray";
			_repairForeground = "Gray";
			_restoreBtnColor = "LightGray";
			_restoreForeground = "Gray";

			GetInstalledVersions();
			FillFoldersLocationList();
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

		public ObservableCollection<StudioLocationListItem> Locations
		{
			get => _locations;

			set
			{
				if (Equals(value, _locations))
				{
					return;
				}
				_locations = value;
				OnPropertyChanged(nameof(Locations));
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

		public ICommand RepairCommand => _repairCommand ??= new CommandHandler(RepairStudio, true);

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

		public string RestoreBtnColor
		{
			get => _restoreBtnColor;

			set
			{
				if (Equals(value, _restoreBtnColor))
				{
					return;
				}
				_restoreBtnColor = value;
				OnPropertyChanged(nameof(RestoreBtnColor));
			}
		}

		public ICommand RestoreCommand => _restoreCommand ??= new CommandHandler(RestoreLocations, true);

		public string RestoreForeground
		{
			get => _restoreForeground;

			set
			{
				if (Equals(value, _restoreForeground))
				{
					return;
				}
				_restoreForeground = value;
				OnPropertyChanged(nameof(RestoreForeground));
			}
		}

		public StudioLocationListItem SelectedLocation
		{
			get => _selectedLocation;
			set
			{
				_selectedLocation = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<StudioVersion> StudioVersionsCollection
		{
			get => _studioVersionsCollection;

			set
			{
				if (Equals(value, _studioVersionsCollection))
				{
					return;
				}
				_studioVersionsCollection = value;
				OnPropertyChanged(nameof(StudioVersionsCollection));
			}
		}

		private bool AnyLocationSelected()
		{
			return Locations.Any(l => l.IsSelected) || RegistryKeyChecked;
		}

		private bool AnyVersionSelected()
		{
			return StudioVersionsCollection.Any(v => v.IsSelected);
		}

		private void CheckAllLocations(bool check)
		{
			foreach (var location in Locations)
			{
				location.IsSelected = check;
			}
		}

		private StudioVersion LatestStudioVersion => StudioVersionsCollection.FirstOrDefault();

		private void FillFoldersLocationList()
		{
			var listOfProperties = new List<string>
			{
				nameof(StudioVersion.AppDataRoamingStudioPath),
				nameof(StudioVersion.AppDataRoamingPluginsPath),
				nameof(StudioVersion.AppDataLocalStudioPath),
				nameof(StudioVersion.AppDataLocalPluginsPath),
				nameof(StudioVersion.ProgramDataPluginsPath),
				nameof(StudioVersion.ProgramDataStudioDataSubfolderPath),
				nameof(StudioVersion.ProjectsXmlPath),
				nameof(StudioVersion.ProjectTemplatesPath),
				nameof(StudioVersion.SdlRegistryKey)
			};
			
			_locations = new ObservableCollection<StudioLocationListItem>();

			foreach (var property in listOfProperties)
			{
				var latestVersionPath = (string)LatestStudioVersion?.GetType().GetProperty(property)?.GetValue(LatestStudioVersion);
				var description = (string)typeof(LocationsDescription).GetProperty(property)?.GetValue(null, null);

				AddLocation(latestVersionPath, description, property);
			}

			AddProjectApiFolderLocation();
			_locations.Move(_locations.Count - 1, _locations.Count - 2);

			foreach (var location in _locations)
			{
				location.PropertyChanged += Location_PropertyChanged;
			}
		}

		/// <summary>
		/// This is needed because this folder is not used by all versions of Studio and so the adding of its path must be done differently
		/// </summary>
		private void AddProjectApiFolderLocation()
		{
			var projectApiFolderPath =
				Path.GetDirectoryName(
					StudioVersionsCollection.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v.ProjectApiPath))?.ProjectApiPath);
			var apiPathDescription = LocationsDescription.ProjectApiPath;

			AddLocation(projectApiFolderPath, apiPathDescription, nameof(StudioVersion.ProjectApiPath));
		}

		private void AddLocation(string path, string description, string alias)
		{
			_locations.Add(new StudioLocationListItem
			{
				DisplayName = path,
				IsSelected = true,
				Description = description,
				Alias = alias
			});
		}

		private void GetInstalledVersions()
		{
			_studioVersionsCollection = new ObservableCollection<StudioVersion>(_versionService.GetInstalledStudioVersions());

			foreach (var studioVersion in _studioVersionsCollection)
			{
				studioVersion.PropertyChanged += StudioVersion_PropertyChanged;
			}
		}

		private string GetMsiName(StudioVersion version)
		{
			var msiName = version.MajorVersion > 15
				? "TradosStudio.msi"
				: $"TranslationStudio{version.LegacyVersion}.msi";
			return msiName;
		}

		private bool IsStudioRunning()
		{
			var processList = Process.GetProcesses();
			var studioProcesses = processList.Where(p => p.ProcessName.Contains(Constants.SDLTradosStudio)).ToList();
			return studioProcesses.Any();
		}

		private void Location_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SetButtonColors();
		}

		public bool RegistryKeyChecked
		{
			get => _registryKeyChecked;
			set
			{
				_registryKeyChecked = value;
				SetButtonColors();
				OnPropertyChanged(nameof(RegistryKeyChecked));
			}
		}

		private List<LocationDetails> GetLocationsForSelectedVersions()
		{
			var allLocations = _persistenceSettings.Load(true);
			var selectedVersions = StudioVersionsCollection.Where(s => s.IsSelected).ToList();
			var locationsForSelectedVersion = new List<LocationDetails>();
			if (selectedVersions.Any())
			{
				foreach (var version in selectedVersions)
				{
					var locations = allLocations.Where(f => f.Version.Equals(version.VersionWithEdition)).ToList();
					locationsForSelectedVersion.AddRange(locations);
				}
				return locationsForSelectedVersion;
			}
			//if nothing is selected, we presume the user wants everything restored
			return allLocations;
		}

		private async void RemoveFromLocations()
		{
			var result = _messageService.ShowConfirmationMessage(Constants.Confirmation, Constants.RemoveMessage);

			if (result == MessageBoxResult.Yes)
			{
				if (!IsStudioRunning())
				{
					var controller = await ShowProgress(Constants.Wait, Constants.RemoveFilesMessage);

					var foldersToClearOrRestore = new List<LocationDetails>();
					var registryToClearOrRestore = new List<LocationDetails>();
					var locations = new List<LocationDetails>();

					var selectedStudioVersions = StudioVersionsCollection.Where(s => s.IsSelected).ToList();
					var selectedLocations = Locations.Where(f => f.IsSelected).ToList();

					if (selectedStudioVersions.Any())
					{
						locations = Paths.GetLocationsFromVersions(selectedLocations.Select(l => l.Alias).ToList(), selectedStudioVersions);

						var registryLocations = locations.TakeWhile(l => l.Alias == nameof(StudioVersion.SdlRegistryKey)).ToList();
						registryToClearOrRestore.AddRange(registryLocations);

						var folderLocations = locations.TakeWhile(l => l.Alias != nameof(StudioVersion.SdlRegistryKey)).ToList();
						foldersToClearOrRestore.AddRange(folderLocations);
					}

					//save local selected locations
					_persistenceSettings.SaveSettings(locations, true);
					await FileManager.BackupFiles(foldersToClearOrRestore);
					await _registryHelper.BackupKeys(registryToClearOrRestore);

					RemoveFromFolders(foldersToClearOrRestore);
					RemoveFromRegistry(registryToClearOrRestore);

					UnselectGrids();
					//to close the message
					await controller.CloseAsync();
				}
				else
				{
					_messageService.ShowWarningMessage(Constants.StudioRunMessage, Constants.CloseStudioRemoveMessage);
				}
			}
		}

		private void RemoveFromRegistry(List<LocationDetails> registryToClearOrRestore)
		{
			try
			{
				_registryHelper.DeleteKeys(registryToClearOrRestore, true);
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

		private void RepairStudio()
		{
			if (!IsStudioRunning())
			{
				if (Directory.Exists(_packageCache))
				{
					var selectedVersions = StudioVersionsCollection.Where(v => v.IsSelected).ToList();
					foreach (var version in selectedVersions)
					{
						RunRepair(version);
					}
				}
			}
			else
			{
				_messageService.ShowWarningMessage(Constants.StudioRunMessage, Constants.CloseStudioRepairMessage);
			}
		}

		private async void RestoreLocations()
		{
			var result =
				_messageService.ShowConfirmationMessage(Constants.Confirmation, Constants.RestoreRemovedFoldersMessage);

			if (result != MessageBoxResult.Yes) return;
			if (!IsStudioRunning())
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
				_messageService.ShowWarningMessage(Constants.StudioRunMessage, Constants.CloseStudioRestoreMessage);
			}
		}

		private async Task RestoreRegistry(List<LocationDetails> locationsToRestore)
		{
			var registryToRestore = locationsToRestore
				.TakeWhile(l => l.Alias == nameof(StudioVersion.SdlRegistryKey)).ToList();
			try
			{
				await _registryHelper.RestoreKeys(registryToRestore);
			}
			catch (Exception e)
			{
				_messageService.ShowWarningMessage(Constants.Warning, string.Format(Resources.NotAllRegistriesCouldBeRestored, e.Message));
			}
		}

		private async Task RestoreFolders(List<LocationDetails> locationsToRestore)
		{
			var foldersToRestore = locationsToRestore
				.TakeWhile(l => l.Alias != nameof(StudioVersion.SdlRegistryKey)).ToList();
			await FileManager.RestoreBackupFiles(foldersToRestore);
		}

		private void RunRepair(StudioVersion version)
		{
			_logger.Info(
				$"Selected Trados executable version: Minor - {version.ExecutableVersion.Minor}, Build - {version.ExecutableVersion.Build}");

			var currentVersionFolder = _versionService.GetPackageCacheCurrentFolder(version.ExecutableVersion,
				version.CacheFolderName, version.Edition.ToLower().Equals("beta"));
			var msiName = GetMsiName(version);
			var moduleDirectoryPath = Path.Combine(currentVersionFolder, "modules");

			_versionService.RunRepairMsi(moduleDirectoryPath, msiName);
		}

		private void SetButtonColors()
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

		private async Task<ProgressDialogController> ShowProgress(string title, string message)
		{
			var controller = await _mainWindow.ShowProgressAsync(title, message);
			controller.SetIndeterminate();
			return controller;
		}

		private void StudioVersion_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SetButtonColors();
		}

		private void UnselectGrids()
		{
			var selectedVersions = StudioVersionsCollection.Where(v => v.IsSelected).ToList();
			foreach (var version in selectedVersions)
			{
				version.IsSelected = false;
			}

			var selectedLocations = Locations.Where(l => l.IsSelected).ToList();
			foreach (var selectedLocation in selectedLocations)
			{
				selectedLocation.IsSelected = false;
			}
			CheckAll = false;
		}
	}
}
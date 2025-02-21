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
		private readonly StudioVersionService _versionService;
		private bool _checkAll;
		private ObservableCollection<StudioLocationListItem> _locations = new ObservableCollection<StudioLocationListItem>();
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
		private ObservableCollection<IStudioVersion> _studioVersionsCollection;
		private bool _registryKeyChecked;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public StudioViewModel(MainWindow mainWindow, StudioVersionService versionService, IMessageService messageService, IRegistryHelper registryHelper)
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

		public ObservableCollection<IStudioVersion> StudioVersionsCollection
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

		private void FillFoldersLocationList()
		{
			Locations?.Clear();
			var listOfProperties = new List<(string, string)>
			{
				(nameof(StudioVersion.GeneralSettingsFolder), "General settings"),
				(nameof(StudioVersion.RoamingPluginsFolder), "Roaming plugins"),
				(nameof(StudioVersion.LocalTradosLogsFolder), "Logs"),
				(nameof(StudioVersion.LocalPluginsFolder), "Local plugins"),
				(nameof(StudioVersion.ProgramDataPluginsFolder), "Program data plugins"),
				(nameof(StudioVersion.ProgramDataUpdatesFolder), "Updates"),
				(nameof(StudioVersion.ProjectsXmlPath), "Project list"),
				(nameof(StudioVersion.ProgramDataProjectTemplatesFolder), "Project templates"),
				(nameof(StudioVersion.SdlRegistryKeys), "Registry keys")
			};
			
			foreach (var studioVersion in StudioVersionsCollection.Where(v => v.IsSelected))
			{
				foreach (var property in listOfProperties)
				{
					var latestVersionPath = (string)studioVersion?.GetType().GetProperty(property.Item1)?.GetValue(studioVersion);
					var description = (string)typeof(LocationsDescription).GetProperty(property.Item1)?.GetValue(null, null);

					AddLocation(latestVersionPath, description, property.Item1, property.Item2);
				}
			}

			AddProjectApiFileLocation();

			foreach (var location in _locations)
			{
				location.PropertyChanged += Location_PropertyChanged;
			}
		}

		/// <summary>
		/// This is needed because this folder is not used by all versions of Studio and so the adding of its path must be done differently
		/// </summary>
		private void AddProjectApiFileLocation()
		{
			var projectApiFolderPaths =
				StudioVersionsCollection.Where(v => v.IsSelected && !string.IsNullOrWhiteSpace(v.ProjectApiPath))?.Select(
					v => v.ProjectApiPath);
			var apiPathDescription = LocationsDescription.ProjectApiPath;

			foreach (var path in projectApiFolderPaths)
			{
				AddLocation(path, apiPathDescription, nameof(StudioVersion.ProjectApiPath), "Project API file");
			}
		}

		private void AddLocation(string path, string description, string alias, string pathName)
		{
			Locations.Add(new StudioLocationListItem
			{
				DisplayName = $"{pathName}: {path}",
				IsSelected = true,
				Description = description,
				Alias = alias
			});
		}

		private void GetInstalledVersions()
		{
			_studioVersionsCollection = new ObservableCollection<IStudioVersion>(_versionService.GetInstalledStudioVersions());

			foreach (var studioVersion in _studioVersionsCollection)
			{
				studioVersion.PropertyChanged += StudioVersion_PropertyChanged;
			}
		}

		private string GetMsiName(IStudioVersion version)
		{
			var msiName = version.MajorVersion > 15
				? "TradosStudio.msi"
				: $"TranslationStudio{version.LegacyVersion}.msi";
			return msiName;
		}

		private (List<IStudioVersion>, List<IStudioVersion>) GetUnchangeableAndChangeableVersions()
		{
			var processList = Process.GetProcesses();

			var studioProcesses = processList.Where(p => p.ProcessName.Contains(Constants.SDLTradosStudio)).ToList();
			var studioProcessesIds = studioProcesses.Select(p => p.MainModule.FileVersionInfo.FileMajorPart).ToList();

			var selectedVersionsIds = GetSelectedVersions();

			var unchangeable = selectedVersionsIds.Where(sv => studioProcessesIds.Contains(sv.MajorVersion)).ToList();
			var changeable = selectedVersionsIds.Where(sv => !unchangeable.Contains(sv)).ToList();

			return (unchangeable, changeable);
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

		private List<LocationDetails> GetLocationsForSelectedVersions(List<IStudioVersion> selectedChangeableVersions)
		{
			var allLocations = _persistenceSettings.Load(true);
			var locationsForSelectedVersion = new List<LocationDetails>();
			if (selectedChangeableVersions.Any())
			{
				foreach (var version in selectedChangeableVersions)
				{
					var locations = allLocations.Where(f => f.Version.Equals(version.VersionWithEdition)).ToList();
					locationsForSelectedVersion.AddRange(locations);
				}
				return locationsForSelectedVersion;
			}
			//if nothing is selected, we presume the user wants everything restored
			return allLocations;
		}

		private List<IStudioVersion> GetSelectedVersions()
		{
			return StudioVersionsCollection.Where(s => s.IsSelected).ToList();
		}

		private async void RemoveFromLocations()
		{
			var result = _messageService.ShowConfirmationMessage(Constants.Confirmation, Constants.RemoveMessage);

			if (result == MessageBoxResult.Yes)
			{
				var (unchangeableVersions, changeableVersions) = GetUnchangeableAndChangeableVersions();
				var controller = await ShowProgress(Constants.Wait, Constants.RemoveFilesMessage);

				var foldersToClearOrRestore = new List<LocationDetails>();
				var registryToClearOrRestore = new List<LocationDetails>();
				var locations = new List<LocationDetails>();

				var selectedLocations = Locations.Where(f => f.IsSelected).ToList();

				if (changeableVersions.Any())
				{
					locations = Paths.GetLocationsFromVersions(selectedLocations.Select(l => l.Alias).ToList(), changeableVersions);

					var registryLocations = locations.Where(l => l.Alias == nameof(StudioVersion.SdlRegistryKeys)).ToList();
					registryToClearOrRestore.AddRange(registryLocations);

					var folderLocations = locations.TakeWhile(l => l.Alias != nameof(StudioVersion.SdlRegistryKeys)).ToList();
					foldersToClearOrRestore.AddRange(folderLocations);
				}

				_persistenceSettings.SaveSettings(locations, true);
				await FileManager.BackupFiles(foldersToClearOrRestore);
				await _registryHelper.BackupKeys(registryToClearOrRestore);

				RemoveFromFolders(foldersToClearOrRestore);
				RemoveFromRegistry(registryToClearOrRestore);

				UnselectGrids();
				await controller.CloseAsync();

				if (unchangeableVersions.Any())
				{
					GetWarningInfo(unchangeableVersions, changeableVersions, out var unchangedString, out var changedString);
					_messageService.ShowWarningMessage(Constants.StudioRunMessage,
						$"{Constants.CloseStudioRemoveMessage}{Environment.NewLine}{unchangedString}{Environment.NewLine}{changedString}");
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
			var (unchangeableVersions, changeableVersions) = GetUnchangeableAndChangeableVersions();

			foreach (var version in changeableVersions)
			{
				RunRepair(version);
			}

			if (unchangeableVersions.Any())
			{
				GetWarningInfo(unchangeableVersions, changeableVersions, out var unchangedString, out var changedString);
				_messageService.ShowWarningMessage(Constants.StudioRunMessage,
					$"{Constants.CloseStudioRepairMessage}{Environment.NewLine}{unchangedString}{Environment.NewLine}{changedString}");
			}
		}

		private async void RestoreLocations()
		{
			var result = _messageService.ShowConfirmationMessage(Constants.Confirmation, Constants.RestoreRemovedFoldersMessage);

			if (result != MessageBoxResult.Yes) return;
			var (unchangeableVersions, changeableVersions) = GetUnchangeableAndChangeableVersions();

			var controller = await ShowProgress(Constants.Wait, Constants.RestoringMessage);
			var locationsToRestore = GetLocationsForSelectedVersions(changeableVersions);

			await RestoreFolders(locationsToRestore);
			await RestoreRegistry(locationsToRestore);

			UnselectGrids();
			await controller.CloseAsync();

			if (unchangeableVersions.Any())
			{
				GetWarningInfo(unchangeableVersions, changeableVersions, out var unchangedString, out var changedString);
				_messageService.ShowWarningMessage(Constants.StudioRunMessage,
					$"{Constants.CloseStudioRestoreMessage}{Environment.NewLine}{unchangedString}{Environment.NewLine}{changedString}");
			}
		}

		private static void GetWarningInfo(List<IStudioVersion> unchangeableVersions, List<IStudioVersion> changeableVersions, out string unchangedString, out string changedString)
		{
			unchangedString =
				$"Currently running affected versions:{Environment.NewLine}{string.Join(Environment.NewLine, unchangeableVersions.Select(s => s.VersionWithEdition).ToList())}";

			var changeable = changeableVersions.Select(s => s.VersionWithEdition).ToList();
			changedString = changeable.Any()
				? $"Changed:{Environment.NewLine}{string.Join(Environment.NewLine, changeable)}"
				: null;
		}

		private async Task RestoreRegistry(List<LocationDetails> locationsToRestore)
		{
			var registryToRestore = locationsToRestore
				.TakeWhile(l => l.Alias == nameof(StudioVersion.SdlRegistryKeys)).ToList();
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
				.TakeWhile(l => l.Alias != nameof(StudioVersion.SdlRegistryKeys)).ToList();
			await FileManager.RestoreBackupFiles(foldersToRestore);
		}

		private void RunRepair(IStudioVersion version)
		{
			_logger.Info(
				$"Selected Trados executable version: Minor - {version.ExecutableVersion.Minor}, Build - {version.ExecutableVersion.Build}");

			var msiName = GetMsiName(version);
			_versionService.RunRepairMsi(version.ProgramDataPackagePath, msiName);
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
			FillFoldersLocationList();
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
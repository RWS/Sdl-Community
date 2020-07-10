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
using Sdl.Community.SdlFreshstart.Services;

namespace Sdl.Community.SdlFreshstart.ViewModel
{
	public class StudioViewModel : BaseModel
	{
		private readonly MainWindow _mainWindow;
		private readonly IMessageService _messageService;
		private readonly string _packageCache = @"C:\ProgramData\Package Cache\SDL";
		private readonly Persistence _persistenceSettings;
		private readonly VersionService _versionService;
		private bool _checkAll;
		private string _folderDescription;
		private ObservableCollection<StudioLocationListItem> _foldersLocations;
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

		public StudioViewModel(MainWindow mainWindow, VersionService versionService, IMessageService messageService)
		{
			_versionService = versionService;
			_messageService = messageService;
			_mainWindow = mainWindow;
			_persistenceSettings = new Persistence();
			_folderDescription = string.Empty;
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

		public string FolderDescription
		{
			get => _folderDescription;

			set
			{
				if (Equals(value, _folderDescription))
				{
					return;
				}
				_folderDescription = value;
				OnPropertyChanged(nameof(FolderDescription));
			}
		}

		public ObservableCollection<StudioLocationListItem> FoldersLocationsCollection
		{
			get => _foldersLocations;

			set
			{
				if (Equals(value, _foldersLocations))
				{
					return;
				}
				_foldersLocations = value;
				OnPropertyChanged(nameof(FoldersLocationsCollection));
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

		public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new CommandHandler(RemoveFiles, true));

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

		public ICommand RepairCommand => _repairCommand ?? (_repairCommand = new CommandHandler(RepairStudio, true));

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

		public ICommand RestoreCommand => _restoreCommand ?? (_restoreCommand = new CommandHandler(RestoreFolders, true));

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
			return FoldersLocationsCollection.Any(l => l.IsSelected);
		}

		private bool AnyVersionSelected()
		{
			return StudioVersionsCollection.Any(v => v.IsSelected);
		}

		private void CheckAllLocations(bool check)
		{
			foreach (var location in FoldersLocationsCollection)
			{
				location.IsSelected = check;
			}
		}

		private void FillFoldersLocationList()
		{
			var listOfProperties = new List<string>
			{
				"AppDataRoamingStudioPath",
				"AppDataRoamingPluginsPath",
				"AppDataLocalStudioPath",
				"AppDataLocalPluginsPath",
				"ProgramDataStudioPath",
				"ProgramDataPluginsPath",
				"ProgramDataStudioDataSubfolderPath",
				"ProjectsXmlPath",
				"ProjectTemplatesPath",
			};
			
			var latestVersion = StudioVersionsCollection.First();
			_foldersLocations = new ObservableCollection<StudioLocationListItem>();

			foreach (var property in listOfProperties)
			{
				var latestVersionPath = (string)latestVersion?.GetType().GetProperty(property)?.GetValue(latestVersion);
				var description = (string)typeof(FoldersDescriptionText).GetProperty(property)?.GetValue(null, null);

				_foldersLocations.Add(new StudioLocationListItem
				{
					DisplayName = latestVersionPath,
					IsSelected = true,
					Description = description,
					Alias = property
				});
			}

			foreach (var location in _foldersLocations)
			{
				location.PropertyChanged += Location_PropertyChanged;
			}
		}

		private void GetInstalledVersions()
		{
			var installedVersions = _versionService.GetInstalledStudioVersions();
			installedVersions.Sort((item1, item2) =>
				item1.MajorVersion < item2.MajorVersion ? 1 :
				item1.MajorVersion > item2.MajorVersion ? -1 : 0);

			_studioVersionsCollection = new ObservableCollection<StudioVersion>(installedVersions);

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
			var lastSelectedItem = sender as StudioLocationListItem;
			var selectedLocations = FoldersLocationsCollection.Where(s => s.IsSelected).ToList();
			if (lastSelectedItem != null)
			{
				if (lastSelectedItem.IsSelected)
				{
					FolderDescription = lastSelectedItem.Description;
				}
				else
				{
					if (selectedLocations.Any())
					{
						FolderDescription = selectedLocations.First().Description;
					}
				}
			}
			if (!selectedLocations.Any())
			{
				FolderDescription = string.Empty;
			}

			SetButtonColors();
		}

		private List<LocationDetails> LocationsForSelectedVersions()
		{
			var allFolders = _persistenceSettings.Load(true);
			var selectedVersions = StudioVersionsCollection.Where(s => s.IsSelected).ToList();
			var locationsForSelectedVersion = new List<LocationDetails>();
			if (selectedVersions.Any())
			{
				foreach (var version in selectedVersions)
				{
					var locations = allFolders.Where(f => f.Version.Equals(version.ShortVersion)).ToList();
					locationsForSelectedVersion.AddRange(locations);
				}
				return locationsForSelectedVersion;
			}
			return allFolders;
		}

		private async void RemoveFiles()
		{
			var result = _messageService.ShowConfirmationMessage(Constants.Confirmation, Constants.RemoveMessage);

			if (result == MessageBoxResult.Yes)
			{
				if (!IsStudioRunning())
				{
					var controller = await ShowProgress();

					var foldersToClearOrRestore = new List<LocationDetails>();

					var selectedStudioVersions = StudioVersionsCollection.Where(s => s.IsSelected).ToList();
					var selectedStudioLocations = FoldersLocationsCollection.Where(f => f.IsSelected).ToList();
					if (selectedStudioVersions.Any())
					{
						var documentsFolderLocation =
							FoldersPath.GetLocationsFromVersions(selectedStudioLocations.Select(l => l.Alias).ToList(), selectedStudioVersions);
						foldersToClearOrRestore.AddRange(documentsFolderLocation);
					}

					//save local selected locations
					_persistenceSettings.SaveSettings(foldersToClearOrRestore, true);
					await FileManager.BackupFiles(foldersToClearOrRestore);

					try
					{
						FileManager.RemoveFromSelectedLocations(foldersToClearOrRestore);
					}
					catch
					{
						_messageService.ShowWarningMessage(Constants.Warning, Constants.FilesNotDeletedMessage);
					}

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

		private async void RestoreFolders()
		{
			var result =
				_messageService.ShowConfirmationMessage(Constants.Confirmation, Constants.RestoreRemovedFoldersMessage);

			if (result == MessageBoxResult.Yes)
			{
				if (!IsStudioRunning())
				{
					var controller = await _mainWindow.ShowProgressAsync(Constants.Wait, Constants.RestoringMessage);
					controller.SetIndeterminate();

					//load saved folders path
					var foldersToRestore = LocationsForSelectedVersions();
					await FileManager.RestoreBackupFiles(foldersToRestore);

					UnselectGrids();
					//to close the message
					await controller.CloseAsync();
				}
				else
				{
					_messageService.ShowWarningMessage(Constants.StudioRunMessage, Constants.CloseStudioRestoreMessage);
				}
			}
		}

		private void RunRepair(StudioVersion version)
		{
			var directoriesPath = new DirectoryInfo(_packageCache).GetDirectories()
				.Where(n => n.Name.Contains(version.CacheFolderName))
				.Select(n => n.FullName).ToList();
			foreach (var directoryPath in directoriesPath)
			{
				var msiName = GetMsiName(version);
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

		private async Task<ProgressDialogController> ShowProgress()
		{
			var controller = await _mainWindow.ShowProgressAsync(Constants.Wait, Constants.RemoveFilesMessage);
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

			var selectedLocations = FoldersLocationsCollection.Where(l => l.IsSelected).ToList();
			foreach (var selectedLocation in selectedLocations)
			{
				selectedLocation.IsSelected = false;
			}
			CheckAll = false;
		}
	}
}
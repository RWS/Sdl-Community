using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Sdl.Community.SdlFreshstart.Helpers;
using Sdl.Community.SdlFreshstart.Model;
using Sdl.Community.SdlFreshstart.Properties;

namespace Sdl.Community.SdlFreshstart.ViewModel
{
    public class StudioViewModel:INotifyPropertyChanged
	{
	    private ObservableCollection<StudioVersionListItem> _studioVersionsCollection;
	    private ObservableCollection<StudioLocationListItem> _foldersLocations;
		private StudioLocationListItem _selectedLocation;
		public event PropertyChangedEventHandler PropertyChanged;
		private string _folderDescription;
		private ICommand _removeCommand;
		private ICommand _repairCommand;
		private ICommand _restoreCommand;
		private readonly MainWindow _mainWindow;
		private readonly string _userName;
		private bool _isRemoveEnabled;
		private bool _isRepairEnabled;
		private bool _checkAll;
		private string _removeBtnColor;
		private string _removeForeground;
		private string _repairBtnColor;
		private string _repairForeground;
		private string _restoreBtnColor;
		private string _restoreForeground;
		private string _packageCache = @"C:\ProgramData\Package Cache\SDL";
		private readonly Persistence _persistenceSettings;

		public StudioViewModel(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			_persistenceSettings = new Persistence();
		    _folderDescription = string.Empty;
			_userName = Environment.UserName;
			_isRemoveEnabled = false;
			_isRepairEnabled = false;
			_checkAll = false;
			_removeBtnColor = "LightGray";
			_removeForeground = "Gray";
			_repairBtnColor = "LightGray";
			_repairForeground = "Gray";
			_restoreBtnColor = "LightGray";
			_restoreForeground = "Gray";
			FillStudioVersionList();
		    FillFoldersLocationList();
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
		private void FillFoldersLocationList()
		{
			_foldersLocations = new ObservableCollection<StudioLocationListItem>
			{
				new StudioLocationListItem
				{
					DisplayName = @"C:\Users\[USERNAME]\AppData\Roaming\SDL\SDL Trados Studio\15.0.0.0",
					IsSelected = true,
					Description = FoldersDescriptionText.AppDataRoamingMajorFull(),
					Alias = "roamingMajorFull"
				},
				new StudioLocationListItem
				{
					DisplayName = @"C:\Users\[USERNAME]\AppData\Local\SDL\SDL Trados Studio\15",
					IsSelected = true,
					Description = FoldersDescriptionText.AppDataLocalMajor(),
					Alias = "localMajor"
				},

				new StudioLocationListItem
				{
					DisplayName = @"C:\Users\[USERNAME]\AppData\Roaming\SDL\SDL Trados Studio\15",
					IsSelected = true,
					Description = FoldersDescriptionText.AppDataRoamingMajor(),
					Alias = "roamingMajor"
				},
				new StudioLocationListItem
				{
					DisplayName = @"C:\Users\[USERNAME]\AppData\Local\SDL\SDL Trados Studio\15.0.0.0",
					IsSelected = true,
					Description = FoldersDescriptionText.AppDataLocalMajorFull(),
					Alias = "localMajorFull"
				},
				new StudioLocationListItem
				{
					DisplayName = @"C:\ProgramData\SDL\SDL Trados Studio\15",
					IsSelected = true,
					Description = FoldersDescriptionText.ProgramData(),
					Alias = "programDataMajor"
				},
				new StudioLocationListItem
				{
					DisplayName = @"C:\ProgramData\SDL\SDL Trados Studio\15.0.0.0",
					IsSelected = true,
					Description = FoldersDescriptionText.ProgramDataFull(),
					Alias = "programDataMajorFull"
				},
				new StudioLocationListItem
				{
					DisplayName = @"C:\ProgramData\SDL\SDL Trados Studio\Studio15",
					IsSelected = true,
					Description = FoldersDescriptionText.ProgramDataVersionNumber(),
					Alias = "programData"
				},
				new StudioLocationListItem
				{
					DisplayName = @"C:\Users\[USERNAME]\Documents\Studio 2019\Projects\projects.xml",
					IsSelected = false,
					Description = FoldersDescriptionText.ProjectsXml(),
					Alias = "projectsXml"
				},
				new StudioLocationListItem
				{
					DisplayName = @"C:\Users\[USERNAME]\Documents\Studio 2019\Project Templates",
					IsSelected = false,
					Description = FoldersDescriptionText.ProjectsTemplates(),
					Alias = "projectTemplates"
				},
				new StudioLocationListItem
				{
					DisplayName = @"C:\Users[USERNAME]\AppData\Roaming\SDL\ProjectApi\15.0.0.0",
					IsSelected = false,
					Description = FoldersDescriptionText.ProjectApi(),
					Alias = "roamingProjectApi"
				}
			};

		    foreach (var location in _foldersLocations)
		    {
				location.PropertyChanged += Location_PropertyChanged;
			}
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


		private void SetButtonColors()
		{
			if (AnyLocationSelected()&&AnyVersionSelected())
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

		private void FillStudioVersionList()
	    {
		    _studioVersionsCollection = new ObservableCollection<StudioVersionListItem>
		    {
				new StudioVersionListItem
				{
					DisplayName = "Studio 2019",
					IsSelected = false,
					MajorVersionNumber = "15",
					MinorVersionNumber = "15",
					FolderName ="Studio15",
					CacheFolderName = "SDLTradosStudio2019"
				},
			    new StudioVersionListItem
			    {
				    DisplayName = "Studio 2017",
				    IsSelected = false,
					MajorVersionNumber = "14",
					MinorVersionNumber = "5",
					FolderName ="Studio5",
					CacheFolderName = "SDLTradosStudio2017"
				},
			    new StudioVersionListItem
			    {
				    DisplayName = "Studio 2015",
				    IsSelected = false,
					MajorVersionNumber = "12",
				    MinorVersionNumber = "4",
					FolderName = "Studio4",
				    CacheFolderName = "SDLTradosStudio2015"
				},
			    new StudioVersionListItem
			    {
				    DisplayName = "Studio 2014",
					MajorVersionNumber = "11",
				    MinorVersionNumber = "3",
					IsSelected = false,
				    FolderName = "Studio3",
				    CacheFolderName = "SDLTradosStudio2014"
				}
		    };
		    foreach (var studioVersion in _studioVersionsCollection)
		    {
				studioVersion.PropertyChanged += StudioVersion_PropertyChanged;
		    }
	    }

		private void StudioVersion_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SetButtonColors();
		}

		public ObservableCollection<StudioVersionListItem> StudioVersionsCollection
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


		public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new CommandHandler(RemoveFiles, true));
		public ICommand RepairCommand => _repairCommand ?? (_repairCommand = new CommandHandler(RepairStudio, true));
		public ICommand RestoreCommand => _restoreCommand ?? (_restoreCommand = new CommandHandler(RestoreFolders, true));

		private async void RestoreFolders()
		{
			var dialog = new MetroDialogSettings
			{
				AffirmativeButtonText = "OK"

			};
			var result =
				await _mainWindow.ShowMessageAsync("Please confirm", "Are you sure you want to restore removed folders?", MessageDialogStyle.AffirmativeAndNegative, dialog);
			if (result == MessageDialogResult.Affirmative)
			{
				if (!IsStudioRunning())
				{
					var controller = await _mainWindow.ShowProgressAsync("Please wait...", "We are restoring selected folders");
					controller.SetIndeterminate();

					//load saved folders path
					var foldersToRestore = LocationsForSelectedVersions();
					await Remove.RestoreBackupFiles(foldersToRestore);

					UnselectGrids();
					//to close the message
					await controller.CloseAsync();
				}
				else
				{
					await _mainWindow.ShowMessageAsync("Studio is running",
						"Please close Trados Studio in order to restore selected folders.", MessageDialogStyle.Affirmative, dialog);
				}
			}
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
					var locations = allFolders.Where(v => v.Version.Equals(version.DisplayName)).ToList();
					locationsForSelectedVersion.AddRange(locations);
				}
				return locationsForSelectedVersion;
			}
			return allFolders;
		}

		private async void RepairStudio()
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
				var dialog = new MetroDialogSettings
				{
					AffirmativeButtonText = "OK"

				};
				await _mainWindow.ShowMessageAsync("Studio is running",
					"Please close Trados Studio in order to repair it.", MessageDialogStyle.Affirmative, dialog);
			}
		
		}

		private void RunRepair(StudioVersionListItem version)
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
					var msiFile = Path.Combine(moduleDirectoryPath,msiName);
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

		private string GetMsiName(StudioVersionListItem version)
		{
			var msiName = string.Format("TranslationStudio{0}.msi", version.MinorVersionNumber);
			return msiName;
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

		private void CheckAllLocations(bool check)
		{
			foreach (var location in FoldersLocationsCollection)
			{
				location.IsSelected = check;
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

		private async void RemoveFiles()
		{
			var dialog = new MetroDialogSettings
			{
				AffirmativeButtonText = "OK"

			};
			var result =
				await _mainWindow.ShowMessageAsync("Please confirm","Are you sure you want to remove this files?",MessageDialogStyle.AffirmativeAndNegative,dialog);
			if (result == MessageDialogResult.Affirmative)
			{
				if (!IsStudioRunning())
				{
					var foldersToClearOrRestore = new List<LocationDetails>();
					var controller = await _mainWindow.ShowProgressAsync("Please wait...", "We are removing selected files");
					controller.SetIndeterminate();

					var selectedStudioVersions = StudioVersionsCollection.Where(s => s.IsSelected).ToList();
					var selectedStudioLocations = FoldersLocationsCollection.Where(f => f.IsSelected).ToList();
					if (selectedStudioVersions.Any())
					{
						var documentsFolderLocation =
							await FoldersPath.GetFoldersPath(_userName, selectedStudioVersions, selectedStudioLocations);
						foldersToClearOrRestore.AddRange(documentsFolderLocation);
					}

					//save local selected locations
					_persistenceSettings.SaveSettings(foldersToClearOrRestore,true);
					await Remove.BackupFiles(foldersToClearOrRestore);

					await Remove.FromSelectedLocations(foldersToClearOrRestore);

					UnselectGrids();
					//to close the message
					await controller.CloseAsync();
				}
				else
				{
					await _mainWindow.ShowMessageAsync("Studio is running",
						"Please close Trados Studio in order to remove selected folders.", MessageDialogStyle.Affirmative, dialog);
				}

			}
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
		private bool IsStudioRunning()
		{
			var processList = Process.GetProcesses();
			var studioProcesses = processList.Where(p => p.ProcessName.Contains("SDLTradosStudio")).ToList();
			return studioProcesses.Any();
		}

		private bool AnyLocationSelected()
		{
			return FoldersLocationsCollection.Any(l => l.IsSelected);
		}

		private bool AnyVersionSelected()
		{
			return StudioVersionsCollection.Any(v => v.IsSelected);
		}

		[NotifyPropertyChangedInvocator]
	    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }
	}
}

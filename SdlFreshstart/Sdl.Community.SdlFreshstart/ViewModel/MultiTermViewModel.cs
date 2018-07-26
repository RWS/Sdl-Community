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
    public class MultiTermViewModel : INotifyPropertyChanged
	{
	    private readonly MainWindow _mainWindow;
		private readonly string _userName;
		private ObservableCollection<MultiTermVersionListItem> _multiTermVersionsCollection;
		private ObservableCollection<MultiTermLocationListItem> _multiTermLocationCollection;
		private MultiTermLocationListItem _selectedLocation;
		private string _packageCache = @"C:\ProgramData\Package Cache\SDL";
		private string _folderDescription;
		private ICommand _removeCommand;
		private ICommand _repairCommand;
		private ICommand _restoreCommand;
		private Persistence _persistence; 
		private string _removeForeground;
		private string _removeBtnColor;
		private string _repairBtnColor;
		private string _repairForeground;
		private bool _isRemoveEnabled;
		private bool _isRestoreEnabled;
		private bool _isRepairEnabled;
		private bool _checkAll;

		public MultiTermViewModel(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
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

		public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new CommandHandler(RemoveFiles, true));
		public ICommand RepairCommand => _repairCommand ?? (_repairCommand = new CommandHandler(RepairMultiTerm, true));
		public ICommand RestoreCommand => _restoreCommand ?? (_restoreCommand = new CommandHandler(RestoreFolders, true));

		private async void RestoreFolders()
		{
			var dialog = new MetroDialogSettings
			{
				AffirmativeButtonText = "OK"

			};
			var result =
				await _mainWindow.ShowMessageAsync("Please confirm", "Are you sure you want to restore this folders?", MessageDialogStyle.AffirmativeAndNegative, dialog);
			if (result == MessageDialogResult.Affirmative)
			{
				if (!MultiTermIsRunning())
				{
					var controller = await _mainWindow.ShowProgressAsync("Please wait...", "We are restoring selected folders");
					controller.SetIndeterminate();

					var foldersToRestore = LocationsForSelectedVersions();
					await Remove.RestoreBackupFiles(foldersToRestore);
					UnselectGrids();
					CheckAll = false;

					//to close the message
					await controller.CloseAsync();
				}
				else
				{
					await _mainWindow.ShowMessageAsync("MultiTerm is running",
						"Please close MultiTerm in order to restore selected folders.", MessageDialogStyle.Affirmative, dialog);
				}
			}
		}

		private async void RepairMultiTerm()
		{
			var dialog = new MetroDialogSettings
			{
				AffirmativeButtonText = "OK"

			};
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
				await _mainWindow.ShowMessageAsync("MultiTerm is running",
					"Please close MultiTerm to repair it.", MessageDialogStyle.Affirmative, dialog);
			}
		}

		private void RunRepair(MultiTermVersionListItem selectedVersion)
		{
			var directoriesPath = new DirectoryInfo(_packageCache).GetDirectories()
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

		private string GetMsiName(MultiTermVersionListItem selectedVersion)
		{
			var msiName = string.Format("MTCore{0}.msi", selectedVersion.MajorVersionNumber);
			return msiName;
		}

		private void FillMultiTermLocationList()
		{
			_multiTermLocationCollection = new ObservableCollection<MultiTermLocationListItem>
			{
			 new MultiTermLocationListItem
				{
					DisplayName = @"C:\Users\[USERNAME]\AppData\Local\SDL\SDL MultiTerm\MultiTerm15",
					IsSelected = false,
					Description = FoldersDescriptionText.MultiTermLocal(),
					Alias = "appDataLocal"
				},new MultiTermLocationListItem
				{
					DisplayName = @"C:\Users\[USERNAME]\AppData\Roaming\SDL\SDL MultiTerm\MultiTerm15",
					IsSelected = false,
					Description =FoldersDescriptionText.MultiTermRoaming(),
					Alias = "appDataRoming"
				}
			};

			foreach (var multiTermLocation in _multiTermLocationCollection)
			{	
				multiTermLocation.PropertyChanged += MultiTermLocation_PropertyChanged;
			}
		}

		private async void RemoveFiles()
		{
			var dialog = new MetroDialogSettings
			{
				AffirmativeButtonText = "OK"

			};
			var result =
				await _mainWindow.ShowMessageAsync("Please confirm", "Are you sure you want to remove this files?", MessageDialogStyle.AffirmativeAndNegative, dialog);
			if (result == MessageDialogResult.Affirmative)
			{
				if (!MultiTermIsRunning())
				{
					var controller = await _mainWindow.ShowProgressAsync("Please wait...", "We are removing selected files");
					controller.SetIndeterminate();

					var foldersToClearOrRestore = new List<LocationDetails>();
					controller.SetIndeterminate();

					var selectedMultiTermVersions = MultiTermVersionsCollection.Where(s => s.IsSelected).ToList();
					var selectedMultiTermLocations = MultiTermLocationCollection.Where(f => f.IsSelected).ToList();
					if (selectedMultiTermVersions.Any())
					{
						var documentsFolderLocation =
							await FoldersPath.GetMultiTermFoldersPath(_userName, selectedMultiTermVersions, selectedMultiTermLocations);
						foldersToClearOrRestore.AddRange(documentsFolderLocation);
					}

					//save settings 
					_persistence.SaveSettings(foldersToClearOrRestore,false);
					
					await Remove.BackupFiles(foldersToClearOrRestore);

					await Remove.FromSelectedLocations(foldersToClearOrRestore);
					
					//to close the message
					await controller.CloseAsync();
				}
				else
				{
					await _mainWindow.ShowMessageAsync("MultiTerm is running",
						"Please close MultiTerm in order to remove selected folders.", MessageDialogStyle.Affirmative, dialog);
				}
				
			}
		}

		private List<LocationDetails> LocationsForSelectedVersions()
		{
			var allFolders = _persistence.Load(false);
			var selectedVersions = MultiTermVersionsCollection.Where(s => s.IsSelected).ToList();
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

		private bool MultiTermIsRunning()
		{
			var processList = Process.GetProcesses();
			var multiTermProcesses = processList.Where(p => p.ProcessName.Contains("MultiTerm")).ToList();
			return multiTermProcesses.Any();
		}

		public MultiTermLocationListItem SelectedLocation
		{
			get => _selectedLocation;
			set
			{
				_selectedLocation = value;
				OnPropertyChanged();
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

		private  void UnselectGrids()
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
		}
		private void MultiTermLocation_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var lastSelectedItem = sender as MultiTermLocationListItem;
			var selectedLocations = MultiTermLocationCollection.Where(s => s.IsSelected).ToList();
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
			SetRemoveBtnColors();
		}

		private void FillMultiTermVersionList()
		{
			_multiTermVersionsCollection = new ObservableCollection<MultiTermVersionListItem>
			{
				new MultiTermVersionListItem
				{
					DisplayName = "MultiTerm 2019",
					IsSelected = false,
					MajorVersionNumber = "15",
					ReleaseNumber = "2019",
					CacheFolderName = "SDLMultiTermDesktop2019"

				},
				new MultiTermVersionListItem
				{
					DisplayName = "MultiTerm 2017",
					IsSelected = false,
					MajorVersionNumber = "14",
					ReleaseNumber = "2017",
					CacheFolderName = "SDLMultiTermDesktop2017"

				},
				new MultiTermVersionListItem
				{
					DisplayName = "MultiTerm 2015",
					IsSelected = false,
					MajorVersionNumber = "12",
					ReleaseNumber = "2015",
					CacheFolderName = "SDLMultiTermDesktop2015"
				},
				new MultiTermVersionListItem
				{
					DisplayName = "MultiTerm 2014",
					MajorVersionNumber = "11",
					IsSelected = false,
					ReleaseNumber = "2014",
					CacheFolderName = "SDLMultiTermDesktop2014"
				}
			};

			foreach (var multiTermVersion in _multiTermVersionsCollection)
			{
				multiTermVersion.PropertyChanged += MultiTermVersion_PropertyChanged;
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
		private void MultiTermVersion_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SetRemoveBtnColors();
		}

		private bool AnyLocationSelected()
		{

			return MultiTermLocationCollection.Any(l => l.IsSelected);

		}
		private void CheckAllLocations(bool check)
		{
			foreach (var location in MultiTermLocationCollection)
			{
				location.IsSelected = check;
			}
		}
		private void SetRemoveBtnColors()
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

		private bool AnyVersionSelected()
		{
			return MultiTermVersionsCollection.Any(v => v.IsSelected);
		}

		public ObservableCollection<MultiTermVersionListItem> MultiTermVersionsCollection
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

		public ObservableCollection<MultiTermLocationListItem> MultiTermLocationCollection
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

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

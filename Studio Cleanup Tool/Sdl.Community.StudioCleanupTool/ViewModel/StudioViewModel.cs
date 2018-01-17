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
using Sdl.Community.StudioCleanupTool.Annotations;
using Sdl.Community.StudioCleanupTool.Helpers;
using Sdl.Community.StudioCleanupTool.Model;
using Sdl.Community.StudioCleanupTool.Views;

namespace Sdl.Community.StudioCleanupTool.ViewModel
{
    public class StudioViewModel:INotifyPropertyChanged
	{
	    private ObservableCollection<StudioVersionListItem> _studioVersionsCollection;
	    private ObservableCollection<StudioLocationListItem> _foldersLocations;
	    public event PropertyChangedEventHandler PropertyChanged;
		private string _folderDescription;
		private ICommand _removeCommand;
		private ICommand _repairCommand;
		private ICommand _restoreCommand;
		private readonly MainWindow _mainWindow;
		private readonly string _userName;
		private bool _isRemoveEnabled;
		private bool _isRestoreEnabled;
		private bool _isRepairEnabled;
		private bool _checkAll;
		private string _removeBtnColor;
		private string _removeForeground;
		private string _repairBtnColor;
		private string _repairForeground;
		private string _restoreBtnColor;
		private string _restoreForeground;
		private string _packageCache = @"C:\ProgramData\Package Cache\SDL";

		private List<LocationDetails> _foldersToClearOrRestore = new List<LocationDetails>();
		public StudioViewModel(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
		    _folderDescription = string.Empty;
			_userName = Environment.UserName;
			_isRemoveEnabled = false;
			_isRestoreEnabled =false;
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
		private void FillFoldersLocationList()
	    {
		    _foldersLocations = new ObservableCollection<StudioLocationListItem>
		    {
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users\[USERNAME]\Documents\14\Projects",
				    IsSelected = false,
				    Description = "Removes projects xml file",
					Alias = "projectsXml"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users\[USERNAME]\Documents\14\Project Templates",
				    IsSelected = false,
				    Description = "Removes project templates",
					Alias = "projectTemplates"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users\[USERNAME]\AppData\Roaming\SDL\SDL Trados Studio\14",
				    IsSelected = false,
				    Description = "Removes the plugins",
					Alias = "roamingMajor"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users[USERNAME]\AppData\Roaming\SDL\ProjectApi\14.0.0.0",
				    IsSelected = false,
				    Description = "Removes the plugins",
				    Alias = "roamingProjectApi"
				},
				new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users\[USERNAME]\AppData\Roaming\SDL\SDL Trados Studio\14.0.0.0",
				    IsSelected = false,
				    Description = "Removes some files",
				    Alias = "roamingMajorFull"
				},
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users\[USERNAME]\AppData\Local\SDL\SDL Trados Studio\14",
				    IsSelected = false,
				    Description = "Removes plugins",
				    Alias = "localMajor"
				},
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users\[USERNAME]\AppData\Local\SDL\SDL Trados Studio\14.0.0.0",
				    IsSelected = false,
				    Description = "Removes files",
					Alias = "localMajorFull"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\ProgramData\SDL\SDL Trados Studio\14",
				    IsSelected = false,
				    Description = "Removes files from program data",
					Alias = "programDataMajor"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\ProgramData\SDL\SDL Trados Studio\14.0.0.0",
				    IsSelected = false,
				    Description = "Removes files",
				    Alias = "programDataMajorFull"
				},
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\ProgramData\SDL\SDL Trados Studio\Studio5",
				    IsSelected = false,
				    Description = "Removes files",
				    Alias = "programData"
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
				RemoveBtnColor = "#99b433";
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
				RepairBtnColor = "#99b433";
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
				await _mainWindow.ShowMessageAsync("Please confirm", "Are you sure you want to restore this folders?", MessageDialogStyle.AffirmativeAndNegative, dialog);
			if (result == MessageDialogResult.Affirmative)
			{
				if (!IsStudioRunning())
				{
					var controller = await _mainWindow.ShowProgressAsync("Please wait...", "We are restoring selected folders");
					controller.SetIndeterminate();

					await Remove.RestoreBackupFiles(_foldersToClearOrRestore);

					UnselectGrids();
					//Set colors for restore btn
					IsRestoreEnabled = false;
					RestoreBtnColor = "LightGray";
					RestoreForeground = "Gray";
					//to close the message
					await controller.CloseAsync();
				}
				else
				{
					await _mainWindow.ShowMessageAsync("Studio in running",
						"Please close Trados Studio in order to remove selected folders.", MessageDialogStyle.Affirmative, dialog);
				}
			}
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
				await _mainWindow.ShowMessageAsync("Studio in running",
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
						Process.Start(msiFile);
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
					_foldersToClearOrRestore.Clear();
					var controller = await _mainWindow.ShowProgressAsync("Please wait...", "We are removing selected files");
					controller.SetIndeterminate();

					var selectedStudioVersions = StudioVersionsCollection.Where(s => s.IsSelected).ToList();
					var selectedStudioLocations = FoldersLocationsCollection.Where(f => f.IsSelected).ToList();
					if (selectedStudioVersions.Any())
					{
						var documentsFolderLocation =
							await FoldersPath.GetFoldersPath(_userName, selectedStudioVersions, selectedStudioLocations);
						_foldersToClearOrRestore.AddRange(documentsFolderLocation);
					}

					await Remove.BackupFiles(_foldersToClearOrRestore);

					await Remove.FromSelectedLocations(_foldersToClearOrRestore);

					IsRestoreEnabled = true;
					RestoreBtnColor = "#99b433";
					RestoreForeground = "WhiteSmoke";
					//to close the message
					await controller.CloseAsync();
				}
				else
				{
					await _mainWindow.ShowMessageAsync("Studio in running",
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

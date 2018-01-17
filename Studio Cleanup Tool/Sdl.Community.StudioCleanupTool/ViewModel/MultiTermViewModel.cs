using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Sdl.Community.StudioCleanupTool.Annotations;
using Sdl.Community.StudioCleanupTool.Helpers;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.ViewModel
{
    public class MultiTermViewModel : INotifyPropertyChanged
	{
	    private readonly MainWindow _mainWindow;
		private readonly string _userName;
		private ObservableCollection<MultiTermVersionListItem> _multiTermVersionsCollection;
		private ObservableCollection<MultiTermLocationListItem> _multiTermLocationCollection;
		private List<StudioDetails> _foldersToClearOrRestore = new List<StudioDetails>();
		private string _packageCache = @"C:\ProgramData\Package Cache\SDL";
		private string _folderDescription;
		private ICommand _removeCommand;
		private ICommand _repairCommand;
		private ICommand _restoreCommand;
		private string _restoreBtnColor;
		private string _restoreForeground;
		private string _removeForeground;
		private string _removeBtnColor;
		private string _repairBtnColor;
		private string _repairForeground;
		private bool _isRemoveEnabled;
		private bool _isRestoreEnabled;
		private bool _isRepairEnabled;

		public MultiTermViewModel(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			_userName = Environment.UserName;
			_folderDescription = string.Empty;
			_isRemoveEnabled = false;
			_isRestoreEnabled = false;
			_isRepairEnabled = false;
			_removeBtnColor = "LightGray";
			_removeForeground = "Gray";
			_restoreBtnColor = "LightGray";
			_restoreForeground = "Gray";
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
						Process.Start(msiFile);
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
					DisplayName = @"C:\ProgramData\Package Cache\SDL\SDLMultiTermDesktop2017\",
					IsSelected = false,
					Description = "first description",
					Alias = "packageCache"
				},
				new MultiTermLocationListItem
				{
					DisplayName = @"C:\Program Files (x86)\SDL\SDL MultiTerm\MultiTerm14\",
					IsSelected = false,
					Description = "second",
					Alias = "programFiles"
				},new MultiTermLocationListItem
				{
					DisplayName = @"C:\Users\[USERNAME]\AppData\Local\SDL\SDL MultiTerm\MultiTerm14\",
					IsSelected = false,
					Description = "Another",
					Alias = "appDataLocal"
				},new MultiTermLocationListItem
				{
					DisplayName = @"C:\Users\[USERNAME]\AppData\Roaming\SDL\SDL MultiTerm\MultiTerm14\",
					IsSelected = false,
					Description = "another description",
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

					_foldersToClearOrRestore.Clear();
					controller.SetIndeterminate();

					var selectedMultiTermVersions = MultiTermVersionsCollection.Where(s => s.IsSelected).ToList();
					var selectedMultiTermLocations = MultiTermLocationCollection.Where(f => f.IsSelected).ToList();
					if (selectedMultiTermVersions.Any())
					{
						var documentsFolderLocation =
							await FoldersPath.GetMultiTermFoldersPath(_userName, selectedMultiTermVersions, selectedMultiTermLocations);
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
					await _mainWindow.ShowMessageAsync("MultiTerm in running",
						"Please close MultiTerm in order to remove selected folders.", MessageDialogStyle.Affirmative, dialog);
				}
				
			}
		}

		private bool MultiTermIsRunning()
		{
			var processList = Process.GetProcesses();
			var multiTermProcesses = processList.Where(p => p.ProcessName.Contains("MultiTerm")).ToList();
			return multiTermProcesses.Any();
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
		private void MultiTermVersion_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SetRemoveBtnColors();
		}

		private bool AnyLocationSelected()
		{

			return MultiTermLocationCollection.Any(l => l.IsSelected);

		}
		private void SetRemoveBtnColors()
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
		private string _folderDescription;
		private ICommand _removeCommand;
		private string _removeForeground;
		private string _removeBtnColor;
		private bool _isRemoveEnabled;

		public MultiTermViewModel(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			_userName = Environment.UserName;
			_folderDescription = string.Empty;
			_isRemoveEnabled = false;
			_removeBtnColor = "LightGray";
			_removeForeground = "Gray";
			FillMultiTermVersionList();
			FillMultiTermLocationList();
		}

		public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new CommandHandler(RemoveFiles, true));
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

					var locationsToClear = new List<string>();
					controller.SetIndeterminate();

					var selectedMultiTermVersions = MultiTermVersionsCollection.Where(s => s.IsSelected).ToList();
					var selectedMultiTermLocations = MultiTermLocationCollection.Where(f => f.IsSelected).ToList();
					if (selectedMultiTermVersions.Any())
					{
						var documentsFolderLocation =
							await FoldersPath.GetMultiTermFoldersPath(_userName, selectedMultiTermVersions, selectedMultiTermLocations);
						locationsToClear.AddRange(documentsFolderLocation);
					}

					await Remove.FromSelectedLocations(locationsToClear);

					UnselectGrids();
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
					ReleaseNumber = "2017"

				},
				new MultiTermVersionListItem
				{
					DisplayName = "MultiTerm 2015",
					IsSelected = false,
					MajorVersionNumber = "12",
					ReleaseNumber = "2015"
				},
				new MultiTermVersionListItem
				{
					DisplayName = "MultiTerm 2014",
					MajorVersionNumber = "11",
					IsSelected = false,
					ReleaseNumber = "2014"
				}
			};

			foreach (var multiTermVersion in _multiTermVersionsCollection)
			{
				multiTermVersion.PropertyChanged += MultiTermVersion_PropertyChanged;
			}
		}

		private void MultiTermVersion_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SetRemoveBtnColors();
		}

		private bool AnyLocationAndVersionSelected()
		{
			var selectedVersions = MultiTermVersionsCollection.Where(v => v.IsSelected).ToList();
			var selectedLocations = MultiTermLocationCollection.Where(l => l.IsSelected).ToList();

			return selectedLocations.Any() && selectedVersions.Any();
		}
		private void SetRemoveBtnColors()
		{
			if (AnyLocationAndVersionSelected())
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

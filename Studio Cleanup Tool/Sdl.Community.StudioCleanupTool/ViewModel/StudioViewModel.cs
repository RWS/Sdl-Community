using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Sdl.Community.StudioCleanupTool.Annotations;
using Sdl.Community.StudioCleanupTool.Helpers;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.ViewModel
{
    public class StudioViewModel:INotifyPropertyChanged
	{
	    private ObservableCollection<StudioVersionListItem> _studioVersionsCollection;
	    private ObservableCollection<StudioLocationListItem> _foldersLocations;
	    public event PropertyChangedEventHandler PropertyChanged;
		private string _folderDescription;
		private ICommand _removeCommand;
		private readonly MainWindow _mainWindow;
		private readonly string _userName;
		private bool _isRemoveEnabled;
		private string _removeBtnColor;
		private string _removeForeground;

		public StudioViewModel(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
		    _folderDescription = string.Empty;
			_userName = Environment.UserName;
			_isRemoveEnabled = false;
			_removeBtnColor = "LightGray";
			_removeForeground = "Gray";
			FillStudioVersionList();
		    FillFoldersLocationList();
	    }
		private void FillFoldersLocationList()
	    {
		    //well need to read the information from a file
		    _foldersLocations = new ObservableCollection<StudioLocationListItem>
		    {
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users\[USERNAME]\Documents\14\Projects\projects.xml",
				    IsSelected = false,
				    Description = "Removes projects xml file",
					Alias = "projectsXml"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users\[USERNAME]\Documents\14\Project Templates\",
				    IsSelected = false,
				    Description = "Removes project templates",
					Alias = "projectTemplates"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users\[USERNAME]\AppData\Roaming\SDL\SDL Trados Studio\14\",
				    IsSelected = false,
				    Description = "Removes the plugins",
					Alias = "roamingMajor"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users\[USERNAME]\AppData\Roaming\SDL\SDL Trados Studio\14.0.0.0\",
				    IsSelected = false,
				    Description = "Removes some files",
				    Alias = "roamingMajorFull"
				},
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users\[USERNAME]\AppData\Local\SDL\SDL Trados Studio\14\",
				    IsSelected = false,
				    Description = "Removes plugins",
				    Alias = "localMajor"
				},
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Users\[USERNAME]\AppData\Local\SDL\SDL Trados Studio\14.0.0.0\",
				    IsSelected = false,
				    Description = "Removes files",
					Alias = "localMajorFull"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\ProgramData\SDL\SDL Trados Studio\14\",
				    IsSelected = false,
				    Description = "Removes files from program data",
					Alias = "programDataMajor"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\ProgramData\SDL\SDL Trados Studio\14.0.0.0\",
				    IsSelected = false,
				    Description = "Removes files",
				    Alias = "programDataMajorFull"
				},
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\ProgramData\SDL\SDL Trados Studio\Studio5\",
				    IsSelected = false,
				    Description = "Removes files",
				    Alias = "programData"
				},
			    new StudioLocationListItem
			    {
				    DisplayName = @"C:\Program Files (x86)\SDL\SDL Trados Studio\Studio5\",
				    IsSelected = false,
				    Description = "Removes files",
					Alias = "programFiles"
			    },
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

			SetRemoveBtnColors();
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
		private void FillStudioVersionList()
	    {
		    _studioVersionsCollection = new ObservableCollection<StudioVersionListItem>
		    {
			    new StudioVersionListItem
			    {
				    DisplayName = "Studio 2017",
				    IsSelected = false,
					MajorVersionNumber = "14",
					FolderName ="Studio5"
			    },
			    new StudioVersionListItem
			    {
				    DisplayName = "Studio 2015",
				    IsSelected = false,
					MajorVersionNumber = "12",
					FolderName = "Studio4"
			    },
			    new StudioVersionListItem
			    {
				    DisplayName = "Studio 2014",
					MajorVersionNumber = "11",
				    IsSelected = false,
				    FolderName = "Studio3"
				}
		    };
		    foreach (var studioVersion in _studioVersionsCollection)
		    {
				studioVersion.PropertyChanged += StudioVersion_PropertyChanged;
		    }
	    }

		private void StudioVersion_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SetRemoveBtnColors();
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
					var controller = await _mainWindow.ShowProgressAsync("Please wait...", "We are removing selected files");
					var locationsToClear = new List<string>();
					controller.SetIndeterminate();

					var selectedStudioVersions = StudioVersionsCollection.Where(s => s.IsSelected).ToList();
					var selectedStudioLocations = FoldersLocationsCollection.Where(f => f.IsSelected).ToList();
					if (selectedStudioVersions.Any())
					{
						var documentsFolderLocation =
							await FoldersPath.GetFoldersPath(_userName, selectedStudioVersions, selectedStudioLocations);
						locationsToClear.AddRange(documentsFolderLocation);
					}

					//await  Remove.FromSelectedLocations(locationsToClear);

					UnselectGrids();
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
		}
		private bool IsStudioRunning()
		{
			var processList = Process.GetProcesses();
			var studioProcesses = processList.Where(p => p.ProcessName.Contains("SDLTradosStudio")).ToList();
			return studioProcesses.Any();
		}

		private bool AnyLocationAndVersionSelected()
		{
			var selectedVersions = StudioVersionsCollection.Where(v => v.IsSelected).ToList();
			var selectedLocations = FoldersLocationsCollection.Where(l => l.IsSelected).ToList();

			return selectedLocations.Any() && selectedVersions.Any();
		}

		[NotifyPropertyChangedInvocator]
	    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }
	}
}

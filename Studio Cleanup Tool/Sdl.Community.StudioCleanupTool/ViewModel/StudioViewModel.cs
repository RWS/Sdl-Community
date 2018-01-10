using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
		public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new CommandHandler(RemoveFiles, true));
		

		public StudioViewModel(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
		    _folderDescription = string.Empty;
			_userName = Environment.UserName;
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
				    DisplayName = @"c:\ProgramData\SDL\SDL Trados Studio\14\",
				    IsSelected = false,
				    Description = "Removes files from program data",
					Alias = "programDataMajor"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"c:\ProgramData\SDL\SDL Trados Studio\14.0.0.0\",
				    IsSelected = false,
				    Description = "Removes files",
				    Alias = "programDataMajorFull"
				},
			    new StudioLocationListItem
			    {
				    DisplayName = @"c:\ProgramData\SDL\SDL Trados Studio\Studio5\",
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
				var selectedLocations = FoldersLocationsCollection.Where(s => s.IsSelected).ToList();
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
				
				//to close the message
				//await controller.CloseAsync();
			}
			

		}

		[NotifyPropertyChangedInvocator]
	    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StudioCleanupTool.Annotations;
using Sdl.Community.StudioCleanupTool.Helpers;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.ViewModel
{
    public class StudioViewModel:INotifyPropertyChanged
	{
	    private ObservableCollection<StudioVersion> _studioVersionsCollection;
	    private ObservableCollection<Location> _foldersLocations;
	    public event PropertyChangedEventHandler PropertyChanged;
		private string _folderDescription;
		private ICommand _removeCommand;
		public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new CommandHandler(RemoveFiles, true));
		

		public StudioViewModel()
	    {
		    _folderDescription = string.Empty;
			FillStudioVersionList();
		    FillFoldersLocationList();
	    }
		private void FillFoldersLocationList()
	    {
		    //well need to read the information from a file
		    _foldersLocations = new ObservableCollection<Location>
		    {
			    new Location
			    {
				    DisplayName = @"c:\Users\[USERNAME]\Documents\[Studio Version]\Projects\projects.xml",
				    IsSelected = false,
				    Description = "Removes projects xml file"
			    },
			    new Location
			    {
				    DisplayName = @"c:\Users\[USERNAME]\Documents\[Studio Version]\Project Templates\",
				    IsSelected = false,
				    Description = "Removes project templates"
			    },
			    new Location
			    {
				    DisplayName = @"c:\Users\[USERNAME]\AppData\Roaming\SDL\SDL Trados Studio\[Studio Major Version]\",
				    IsSelected = false,
				    Description = "Removes the plugins"
			    },
			    new Location
			    {
				    DisplayName = @"c:\Users\[USERNAME]\AppData\Roaming\SDL\SDL Trados Studio\[Studio Major Version].0.0.0\",
				    IsSelected = false,
				    Description = "Removes some files"
			    },
			    new Location
			    {
				    DisplayName = @"c:\Users\[USERNAME]\AppData\Local\SDL\SDL Trados Studio\[Studio Major Version]\",
				    IsSelected = false,
				    Description = "Removes plugins"
			    },
			    new Location
			    {
				    DisplayName = @"c:\Users\[USERNAME]\AppData\Local\SDL\SDL Trados Studio\[Studio Major Version].0.0.0\",
				    IsSelected = false,
				    Description = "Removes files"
			    },
			    new Location
			    {
				    DisplayName = @"c:\ProgramData\SDL\SDL Trados Studio\[Studio Major Version]\",
				    IsSelected = false,
				    Description = "Removes files from program data"
			    },
			    new Location
			    {
				    DisplayName = @"c:\ProgramData\SDL\SDL Trados Studio\[Studio Major Version].0.0.0\",
				    IsSelected = false,
				    Description = "Removes files"
			    },
			    new Location
			    {
				    DisplayName = @"c:\ProgramData\SDL\SDL Trados Studio\[Studio Version]\",
				    IsSelected = false,
				    Description = "Removes files"
			    },
			    new Location
			    {
				    DisplayName = @"c:\Program Files (x86)\SDL\SDL Trados Studio\[Studio Version]\",
				    IsSelected = false,
				    Description = "Removes files"
			    },
		    };

		    foreach (var location in _foldersLocations)
		    {
				location.PropertyChanged += Location_PropertyChanged;
			}
	    }

		private void Location_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var lastSelectedItem = sender as Location;
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
		    _studioVersionsCollection = new ObservableCollection<StudioVersion>
		    {
			    new StudioVersion
			    {
				    DisplayName = "Studio 2014",
				    IsSelected = false,
				    //FullVersionNumber = "11.0.0.0",
				    //VersionNumber = "11" //need to check version number
			    },
			    new StudioVersion
			    {
				    DisplayName = "Studio 2015",
				    IsSelected = false,
				    //FullVersionNumber = "12.0.0.0",
			    },
			    new StudioVersion
			    {
				    DisplayName = "Studio 2017",
				    IsSelected = false
			    }
		    };
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

	    public ObservableCollection<Location> FoldersLocationsCollection
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
		private void RemoveFiles()
		{
			var selectedLocations = FoldersLocationsCollection.Where(s => s.IsSelected).ToList();

		}

		[NotifyPropertyChangedInvocator]
	    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Annotations;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.ViewModel
{
    public class StudioViewModel:INotifyPropertyChanged
	{
	    private ObservableCollection<StudioVersion> _studioVersionsCollection;
	    private ObservableCollection<Location> _foldersLocations;
	    public event PropertyChangedEventHandler PropertyChanged;
		private string _folderDescription;

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
				OnPropertyChanged(nameof(FolderDescription));
			}
		}
		
		[NotifyPropertyChangedInvocator]
	    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }
	}
}

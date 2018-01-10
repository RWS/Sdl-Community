using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class StudioViewModel:INotifyPropertyChanged
	{
	    private ObservableCollection<StudioVersionListItem> _studioVersionsCollection;
	    private ObservableCollection<StudioLocationListItem> _foldersLocations;
	    public event PropertyChangedEventHandler PropertyChanged;
		private string _folderDescription;
		private ICommand _removeCommand;
		private MainWindow _mainWindow;
		public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new CommandHandler(RemoveFiles, true));
		

		public StudioViewModel(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
		    _folderDescription = string.Empty;
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
				    DisplayName = @"c:\Users\[USERNAME]\Documents\14\Projects\projects.xml",
				    IsSelected = false,
				    Description = "Removes projects xml file"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"c:\Users\[USERNAME]\Documents\14\Project Templates\",
				    IsSelected = false,
				    Description = "Removes project templates"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"c:\Users\[USERNAME]\AppData\Roaming\SDL\SDL Trados Studio\14\",
				    IsSelected = false,
				    Description = "Removes the plugins"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"c:\Users\[USERNAME]\AppData\Roaming\SDL\SDL Trados Studio\14.0.0.0\",
				    IsSelected = false,
				    Description = "Removes some files"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"c:\Users\[USERNAME]\AppData\Local\SDL\SDL Trados Studio\14\",
				    IsSelected = false,
				    Description = "Removes plugins"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"c:\Users\[USERNAME]\AppData\Local\SDL\SDL Trados Studio\14.0.0.0\",
				    IsSelected = false,
				    Description = "Removes files"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"c:\ProgramData\SDL\SDL Trados Studio\14\",
				    IsSelected = false,
				    Description = "Removes files from program data"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"c:\ProgramData\SDL\SDL Trados Studio\14.0.0.0\",
				    IsSelected = false,
				    Description = "Removes files"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"c:\ProgramData\SDL\SDL Trados Studio\14\",
				    IsSelected = false,
				    Description = "Removes files"
			    },
			    new StudioLocationListItem
			    {
				    DisplayName = @"c:\Program Files (x86)\SDL\SDL Trados Studio\14\",
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
				    //FullVersionNumber = "11.0.0.0",
				    //VersionNumber = "11" //need to check version number
			    },
			    new StudioVersionListItem
			    {
				    DisplayName = "Studio 2015",
				    IsSelected = false,
				    //FullVersionNumber = "12.0.0.0",
			    },
			    new StudioVersionListItem
			    {
				    DisplayName = "Studio 2014",
				    IsSelected = false
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
				controller.SetIndeterminate();

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

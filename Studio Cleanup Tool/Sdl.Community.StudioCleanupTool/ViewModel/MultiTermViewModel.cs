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
    public class MultiTermViewModel : INotifyPropertyChanged
	{
	    private MainWindow _mainWindow;
		private ObservableCollection<MultiTermVersionListItem> _multiTermVersionsCollection;
		private ObservableCollection<MultiTermLocationListItem> _multiTermLocationCollection;
		private string _folderDescription;
		private ICommand _removeCommand;

		public MultiTermViewModel(MainWindow mainWindow)
		{
			_mainWindow = mainWindow;
			_folderDescription = string.Empty;
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
					DisplayName = @"c:\ProgramData\Package Cache\SDL\SDLMultiTermDesktop2017\",
					IsSelected = false,
					Description = "first description"
				},
				new MultiTermLocationListItem
				{
					DisplayName = @"c:\Program Files (x86)\SDL\SDL MultiTerm\MultiTerm14\",
					IsSelected = false,
					Description = "second"
				},new MultiTermLocationListItem
				{
					DisplayName = @"c:\Users\[USERNAME]\AppData\Local\SDL\SDL MultiTerm\MultiTerm14\",
					IsSelected = false,
					Description = "Another"
				},new MultiTermLocationListItem
				{
					DisplayName = @"c:\Users\[USERNAME]\AppData\Roaming\SDL\SDL MultiTerm\MultiTerm14\",
					IsSelected = false,
					Description = "another description"
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
				var selectedLocations = MultiTermLocationCollection.Where(s => s.IsSelected).ToList();
				var controller = await _mainWindow.ShowProgressAsync("Please wait...", "We are removing selected files");
				controller.SetIndeterminate();

				//to close the message
				//await controller.CloseAsync();
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
		}

		private void FillMultiTermVersionList()
		{
			_multiTermVersionsCollection = new ObservableCollection<MultiTermVersionListItem>
			{
				new MultiTermVersionListItem
				{
					DisplayName = "MultiTerm 2017",
					IsSelected = false,
					MajorVersionNumber = "14"

				},
				new MultiTermVersionListItem
				{
					DisplayName = "MultiTerm 2015",
					IsSelected = false,
					MajorVersionNumber = "12"
				},
				new MultiTermVersionListItem
				{
					DisplayName = "MultiTerm 2014",
					MajorVersionNumber = "11",
					IsSelected = false
				}
			};
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

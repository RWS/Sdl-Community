using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using StudioStyles.Commands;
using StudioStyles.Model;

namespace StudioStyles.ViewModel
{
	public class MainWindowViewModel:BaseModel
	{
		private ObservableCollection<Plugin> _pluginsCollection;
		private string _searchWatermarkText;
		private string _searchText;
		private string _uri;
		private ICommand _clearCommand;
		private ICommand _dragDropCommand;
		private ICommand _windowLoadedCommand;
		public MainWindowViewModel()
		{
			_searchWatermarkText = "Andrea";
			_pluginsCollection = new ObservableCollection<Plugin>
			{
				new Plugin
				{
					PluginName = "Subtitling",
					StudioVersion = "Studio 2019"
				},
				new Plugin
				{
					PluginName = "IATE Real-time Terminology",
					StudioVersion = "Studio 2017,2019"
				},
				new Plugin
				{
					PluginName = "SDL BeGlobal (NMT)",
					StudioVersion = "Studio 2015, 2017, 2019"
				},
				new Plugin
				{
					PluginName = "MT Comparison",
					StudioVersion = "Studio 2017, 2019"
				},
			};
			_uri = "https://google.com";
		}

		public ObservableCollection<Plugin> PluginsCollection
		{
			get => _pluginsCollection;
			set
			{
				_pluginsCollection = value;
				OnPropertyChanged(nameof(PluginsCollection));
			}
		}

		public string SearchWatermarkText
		{
			get => _searchWatermarkText;
			set
			{
				_searchWatermarkText = value;
				OnPropertyChanged(nameof(SearchWatermarkText));
			}
		}
		public string SearchText
		{
			get => _searchText;
			set
			{
				_searchText = value;
				OnPropertyChanged(nameof(SearchText));
			}
		}
		public string Uri
		{
			get => _uri;
			set
			{
				_uri = value;
				OnPropertyChanged(nameof(Uri));
			}
		}
		public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new CommandHandler(Clear, true));

		#region behaviours

		public ICommand WindowLoadedCommand =>_windowLoadedCommand ?? (_windowLoadedCommand = new CommandHandler(WindowLoaded, true));

		private void WindowLoaded()
		{
		}
		public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new RelayCommand(DragAndDrop));

		private void DragAndDrop(object parameter)
		{
			if (parameter == null || !(parameter is DragEventArgs eventArgs)) return;

			var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
			if (fileDrop is string[] filesOrDirectories && filesOrDirectories.Length > 0)
			{
				foreach (var fullPath in filesOrDirectories)
				{
					var fileAttributes = File.GetAttributes(fullPath);
					if (fileAttributes.HasFlag(FileAttributes.Directory))
					{
						//is directory
					}
					//is file
				}
			}
		}

		#endregion

		private void Clear()
		{
			SearchText = string.Empty;
			SearchWatermarkText = "Andrea";
		}
	}
}

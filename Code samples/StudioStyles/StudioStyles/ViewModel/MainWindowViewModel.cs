using System.Collections.ObjectModel;
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
		private ICommand _clearCommand;

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
		public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new CommandHandler(Clear, true));

		private void Clear()
		{
			SearchText = string.Empty;
			SearchWatermarkText = "Andrea";
		}
	}
}

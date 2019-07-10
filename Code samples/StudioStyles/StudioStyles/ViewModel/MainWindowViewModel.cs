using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudioStyles.Model;

namespace StudioStyles.ViewModel
{
	public class MainWindowViewModel:BaseModel
	{
		private ObservableCollection<Plugin> _pluginsCollection;

		public MainWindowViewModel()
		{
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
	}
}

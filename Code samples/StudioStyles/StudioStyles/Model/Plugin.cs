namespace StudioStyles.Model
{
	public class Plugin:BaseModel
	{
		private string _pluginName;
		private string _studioVersion;

		public string PluginName
		{
			get => _pluginName;
			set
			{
				_pluginName = value;
				OnPropertyChanged(nameof(PluginName));
			}
		}

		public string StudioVersion
		{
			get => _studioVersion;
			set
			{
				_studioVersion = value;
				OnPropertyChanged(StudioVersion);
			}
		}
	}
}

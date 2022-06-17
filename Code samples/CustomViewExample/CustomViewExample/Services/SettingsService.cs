using System.IO;
using CustomViewExample.Model;
using Newtonsoft.Json;

namespace CustomViewExample.Services
{
	public class SettingsService
	{
		private readonly CustomViewPathInfo _pathInfo;
		
		public SettingsService(CustomViewPathInfo pathInfo)
		{
			_pathInfo = pathInfo;
		}

		public CustomViewSettings GetSettings()
		{
			if (File.Exists(_pathInfo.SettingsFilePath))
			{
				var json = File.ReadAllText(_pathInfo.SettingsFilePath);
				return JsonConvert.DeserializeObject<CustomViewSettings>(json);
			}

			return new CustomViewSettings();
		}

		public void SaveSettings(CustomViewSettings settings)
		{
			File.WriteAllText(_pathInfo.SettingsFilePath, JsonConvert.SerializeObject(settings));
		}
	}
}

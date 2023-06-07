using System.IO;
using InterpretBank.SettingsService;
using Newtonsoft.Json;

namespace InterpretBank.PersistenceService
{
	public class PersistenceService2
	{
		public PersistenceService2(string path)
		{
			SettingsFilepath = path;
		}

		private string SettingsFilepath { get; }

		public Settings GetSettings()
		{
			return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFilepath));
		}

		public void SaveSettings(Settings settings)
		{
			if (!File.Exists(SettingsFilepath))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilepath));
				using var file = File.Create(SettingsFilepath);
			}
			File.WriteAllText(SettingsFilepath, JsonConvert.SerializeObject(settings));
		}
	}
}
using System;
using System.IO;
using InterpretBank.SettingsService;
using Newtonsoft.Json;

namespace InterpretBank.PersistenceService
{
	public static class PersistenceService
	{
		private static string _settingsPath;

		public static string SettingsPath
		{
			get => _settingsPath;
			set => _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				$@"Trados AppStore\InterpretBank\{value}.json");
		}

		public static Settings GetSettings(string settingsId)
		{
			SettingsPath = settingsId;
			if (!File.Exists(SettingsPath))
				SaveSettings(new Settings(), settingsId);
			var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsPath));
			settings.SettingsId = settingsId;

			return settings;
		}

		public static void SaveSettings(Settings settings, string settingsId)
		{
			SettingsPath = settingsId;

			if (!File.Exists(SettingsPath))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
				using var file = File.Create(SettingsPath);
			}

			File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(settings));
		}
	}
}
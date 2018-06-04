using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.TmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.Helpers
{
	public static class SettingsMethods
	{
		public static bool UserAgreed()
		{
			//if (File.Exists(Constants.SettingsFilePath))
			//{
			//	var json = File.ReadAllText(Constants.SettingsFilePath);
			//	var settings = JsonConvert.DeserializeObject<Settings>(json);
			//	if (settings != null)
			//	{
			//		return settings.Accepted;
			//	}
				
			//}
			//return false;
			var settings = GetSettings();
			return settings.Accepted;
		}

		public static bool DefaultRulesAlreadyAdded()
		{
			var settings = GetSettings();
			return settings.AlreadyAddedDefaultRules;
		}

		public static ObservableCollection<Rule> GetRules()
		{
			var settings = GetSettings();
			return settings.Rules;
		}

		public static Settings GetSettings()
		{
			if (File.Exists(Constants.SettingsFilePath))
			{
				var json = File.ReadAllText(Constants.SettingsFilePath);
				var settings = JsonConvert.DeserializeObject<Settings>(json);
				return settings;
			}
			else
			{
				//create settings file
				var file=File.Create(Constants.SettingsFilePath);
				file.Close();

				var settings = new Settings
				{
					Accepted = false,
					Rules = new ObservableCollection<Rule>(),
					AlreadyAddedDefaultRules = false
				};

				File.WriteAllText(Constants.SettingsFilePath, JsonConvert.SerializeObject(settings));
				return settings;
			}
		}

		public static void  SaveSettings(Settings settings)
		{
			File.WriteAllText(Constants.SettingsFilePath, JsonConvert.SerializeObject(settings));
		}
	}
}

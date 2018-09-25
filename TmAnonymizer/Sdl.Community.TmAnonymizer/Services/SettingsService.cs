using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sdl.Community.SdlTmAnonymizer.Model;

namespace Sdl.Community.SdlTmAnonymizer.Services
{
	public class SettingsService
	{
		public SettingsService(PathInfo pathInfo)
		{
			PathInfo = pathInfo;
		}

		public PathInfo PathInfo { get; }

		public bool UserAgreed()
		{
			var settings = GetSettings();
			return settings.Accepted;
		}

		public bool DefaultRulesAlreadyAdded()
		{
			var settings = GetSettings();
			return settings.AlreadyAddedDefaultRules;
		}

		public List<Rule> GetRules()
		{
			var settings = GetSettings();
			return settings.Rules;
		}

		public List<Rule> GetDefaultRules()
		{
			if (!File.Exists(PathInfo.DefaultRulesFilePath))
			{
				return new List<Rule>();
			}

			var json = File.ReadAllText(PathInfo.DefaultRulesFilePath);
			var settings = JsonConvert.DeserializeObject<Settings>(json);
			return settings.Rules;
		}

		public List<TmFile> GetTmFiles()
		{
			var settings = GetSettings();
			return settings.TmFiles;
		}

		public Settings GetSettings()
		{
			Settings settings;
			if (File.Exists(PathInfo.SettingsFilePath))
			{
				var json = File.ReadAllText(PathInfo.SettingsFilePath);
				settings = JsonConvert.DeserializeObject<Settings>(json);
				return settings;
			}

			//create settings file
			var file = File.Create(PathInfo.SettingsFilePath);
			file.Close();

			settings = new Settings();
			File.WriteAllText(PathInfo.SettingsFilePath, JsonConvert.SerializeObject(new Settings()));

			return settings;
		}

		public void SaveSettings(Settings settings)
		{
			File.WriteAllText(PathInfo.SettingsFilePath, JsonConvert.SerializeObject(settings));
		}

		public void AddDefaultRules()
		{
			var settings = GetSettings();

			if (settings.AlreadyAddedDefaultRules)
			{
				return;
			}

			settings.AlreadyAddedDefaultRules = true;
			settings.Rules = GetDefaultRules();
			SaveSettings(settings);
		}
	}
}

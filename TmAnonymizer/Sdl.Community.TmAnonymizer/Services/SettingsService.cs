using System;
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
			UncheckAllTMs();			
		}
				
		public PathInfo PathInfo { get; }

		public string GetLogReportPath()
		{
			var settings = GetSettings();
			if (String.IsNullOrEmpty(settings.LogsFullPath) || !Directory.Exists(settings.LogsFullPath))
			{
				settings.LogsFullPath = PathInfo.LogsFullPath;
				SaveSettings(settings);
			}

			return settings.LogsFullPath;
		}

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
			// TODO: confirm if we should better manage provide these default settings from an external resource
			return new List<Rule>
			{
				new Rule
				{
					Id = Guid.NewGuid().ToString(),
					Description = "Email addresses",
					Name = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b"
				},
				new Rule
				{
					Id = Guid.NewGuid().ToString(),
					Description = "PCI (Payment Card Industry)",
					Name = @"\b(?:\d[ -]*?){13,16}\b"
				},
				new Rule
				{
					Id = Guid.NewGuid().ToString(),
					Description = "IP4 Address",
					Name = @"\b(?<![:.\w])(?:[A-F0-9]{1,4}:){7}[A-F0-9]{1,4}(?![:.\w])\b"
				},
				new Rule
				{
					Id = Guid.NewGuid().ToString(),
					Description = "MAC Address",
					Name = @"\b[0-9A-F]{2}([-:]?)(?:[0-9A-F]{2}\1){4}[0-9A-F]{2}\b"
				},
				new Rule
				{
					Id = Guid.NewGuid().ToString(),
					Description = "UK National Insurance Number",
					Name = @"\b[A-Z]{2}\s\d{2}\s\d{2}\s\d{2}\s[A-Z]\b"
				},
				new Rule
				{
					Id = Guid.NewGuid().ToString(),
					Description = "Social Security Numbers",
					Name = @"\b(?!000)(?!666)[0-8][0-9]{2}[- ](?!00)[0-9]{2}[- ](?!0000)[0-9]{4}\b"
				}
			};
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

				AddDefaultRules(settings);

				if (String.IsNullOrEmpty(settings.BackupFullPath) || !Directory.Exists(settings.BackupFullPath))
				{
					settings.BackupFullPath = PathInfo.BackupFullPath;
				}

				if (String.IsNullOrEmpty(settings.LogsFullPath) || !Directory.Exists(settings.LogsFullPath))
				{
					settings.LogsFullPath = PathInfo.LogsFullPath;
				}

				return settings;
			}

			//create settings file
			var file = File.Create(PathInfo.SettingsFilePath);
			file.Close();

			settings = new Settings();
			File.WriteAllText(PathInfo.SettingsFilePath, JsonConvert.SerializeObject(settings));

			return settings;
		}

		public void SaveSettings(Settings settings)
		{			
			File.WriteAllText(PathInfo.SettingsFilePath, JsonConvert.SerializeObject(settings));
		}

		public void AddDefaultRules(Settings settings)
		{			
			if (settings.AlreadyAddedDefaultRules)
			{
				return;
			}

			settings.AlreadyAddedDefaultRules = true;
			settings.Rules = GetDefaultRules();
			SaveSettings(settings);
		}

		public string GetDateTimeString()
		{
			var dt = DateTime.Now;
			return dt.Year +
			       dt.Month.ToString().PadLeft(2, '0') +
			       dt.Day.ToString().PadLeft(2, '0') +
			       "T" +
			       dt.Hour.ToString().PadLeft(2, '0') +
			       dt.Minute.ToString().PadLeft(2, '0') +
			       dt.Second.ToString().PadLeft(2, '0');
		}

		private void UncheckAllTMs()
		{
			var settings = GetSettings();
			var tmFiles = GetTmFiles();
			foreach (var tmFile in tmFiles)
			{
				tmFile.IsSelected = false;

				if (!String.IsNullOrEmpty(tmFile.CachePath) && !File.Exists(tmFile.CachePath))
				{
					tmFile.CachePath = String.Empty;
				}
			}

			settings.TmFiles = tmFiles;
			SaveSettings(settings);
		}
	}
}

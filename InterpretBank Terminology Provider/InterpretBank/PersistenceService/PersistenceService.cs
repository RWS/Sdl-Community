using System;
using System.IO;
using InterpretBank.Model;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using InterpretBank.SettingsService;
using InterpretBank.Studio;
using Newtonsoft.Json;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using System.Linq;

namespace InterpretBank.PersistenceService
{
	public class PersistenceService
	{
		private string _settingsPath;

		public string SettingsPath
		{
			get => _settingsPath;
			set => _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				$@"Trados AppStore\InterpretBank\{value}.json");
		}

		public Settings GetSettings(string settingsId)
		{
			SettingsPath = settingsId;
			if (!File.Exists(SettingsPath))
				SaveSettings(new Settings(), settingsId);
			var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsPath));
			settings.SettingsId = settingsId;

			return settings;
		}
        public string GetSettingsId() => (Guid.NewGuid().ToString());
        public Settings GetSettingsForSelectedProject()
		{
            var selectedProjects = StudioContext.ProjectsController.SelectedProjects;

            var selectedProject = selectedProjects != null
                ? selectedProjects.LastOrDefault()
                : StudioContext.ProjectsController.CurrentProject;

            var termbaseConfiguration = selectedProject?.GetTermbaseConfiguration();

            if (!termbaseConfiguration?.Termbases?.Any() ?? true) return null;

            var settingsXml = termbaseConfiguration.Termbases[0].SettingsXML;
            var serializer = new XmlSerializer(typeof(TermbaseSettings));

            using TextReader reader = new StringReader(settingsXml);
            var termbaseSettingsUri = ((TermbaseSettings)serializer.Deserialize(reader)).Path;

            var termbaseSettingsPath = Regex.Split(termbaseSettingsUri, "//")[1].Split(['.'])[0];
            var settingsId = termbaseSettingsPath.TrimStart('/');

            SettingsPath = settingsId;

            if (!File.Exists(SettingsPath))
                SaveSettings(new Settings(), settingsId);
            var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsPath));
            settings.SettingsId = settingsId;

            return settings;
        }

		public void SaveSettings(Settings settings, string settingsId)
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
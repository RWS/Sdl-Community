using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdl.Community.Structures.Configuration
{
	[Serializable]
	public class ApplicationPaths : ICloneable
	{
		public string ApplicationSettingsPath { get; set; }

		public string ApplicationTrackChangesPath { get; set; }
		public string ApplicationTrackChangesReportPath { get; set; }

		public string ApplicationMyDocumentsPath { get; set; }
		public string ApplicationMyDocumentsDatabasePath { get; set; }
		public string ApplicationMyDocumentsDatabaseSettingsPath { get; set; }
		public string ApplicationMyDocumentsDatabaseProjectsPath { get; set; }

		public string ApplicationBackupDatabasePath { get; set; }

		const string MyDocumentsPath = "MyDocumentsPath";
		const string ApplicationDataPath = "ApplicationDataPath";

		public ApplicationPaths()
		{
			var location = Assembly.GetExecutingAssembly().Location;
			var unpackedFolder = location.Substring(0, location.LastIndexOf("\\", StringComparison.Ordinal));

			var assembly = Path.Combine(unpackedFolder, "Qualitivity.dll");
			var config = ConfigurationManager.OpenExeConfiguration(assembly);

			var myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

			if (config.AppSettings?.Settings != null)
			{
				var myDocumentsPathCustom = GetConfigValue(config, MyDocumentsPath);
				if (myDocumentsPathCustom != null)
				{
					myDocumentsPath = myDocumentsPathCustom;
				}

				var applicationDataPathCustom = GetConfigValue(config, ApplicationDataPath);
				if (applicationDataPathCustom != null)
				{
					applicationDataPath = applicationDataPathCustom;
				}
			}

			ApplicationMyDocumentsPath = Path.Combine(myDocumentsPath, "Qualitivity");
			if (!Directory.Exists(ApplicationMyDocumentsPath))
				Directory.CreateDirectory(ApplicationMyDocumentsPath);

			ApplicationMyDocumentsDatabasePath = Path.Combine(ApplicationMyDocumentsPath, "Database");
			if (!Directory.Exists(ApplicationMyDocumentsDatabasePath))
				Directory.CreateDirectory(ApplicationMyDocumentsDatabasePath);

			ApplicationMyDocumentsDatabaseSettingsPath = Path.Combine(ApplicationMyDocumentsDatabasePath, "Settings.sqlite");
			ApplicationMyDocumentsDatabaseProjectsPath = Path.Combine(ApplicationMyDocumentsDatabasePath, "Projects.sqlite");

			ApplicationBackupDatabasePath = Path.Combine(ApplicationMyDocumentsDatabasePath, "Backups");
			if (!Directory.Exists(ApplicationBackupDatabasePath))
				Directory.CreateDirectory(ApplicationBackupDatabasePath);

			ApplicationSettingsPath = Path.Combine(applicationDataPath, "Qualitivity");
			if (!Directory.Exists(ApplicationSettingsPath))
				Directory.CreateDirectory(ApplicationSettingsPath);

			ApplicationTrackChangesPath = Path.Combine(ApplicationSettingsPath, "Track.Changes");
			if (!Directory.Exists(ApplicationTrackChangesPath))
				Directory.CreateDirectory(ApplicationTrackChangesPath);

			ApplicationTrackChangesReportPath = Path.Combine(ApplicationTrackChangesPath, "Reports");
			if (!Directory.Exists(ApplicationTrackChangesReportPath))
				Directory.CreateDirectory(ApplicationTrackChangesReportPath);
		}

		private static string GetConfigValue(System.Configuration.Configuration config, string propertyName)
		{
			if (config.AppSettings.Settings.AllKeys.Any(a => a == propertyName))
			{
				var value = config.AppSettings.Settings[propertyName].Value;
				if (value != null && Directory.Exists(value))
				{
					return value;
				}
			}

			return null;
		}

		public object Clone()
		{
			var applicationPaths = new ApplicationPaths
			{
				ApplicationSettingsPath = ApplicationSettingsPath,
				ApplicationTrackChangesPath = ApplicationTrackChangesPath,
				ApplicationTrackChangesReportPath = ApplicationTrackChangesReportPath,
				ApplicationMyDocumentsPath = ApplicationMyDocumentsPath,
				ApplicationMyDocumentsDatabasePath = ApplicationMyDocumentsDatabasePath,
				ApplicationBackupDatabasePath = ApplicationBackupDatabasePath
			};






			return applicationPaths;
		}
	}
}

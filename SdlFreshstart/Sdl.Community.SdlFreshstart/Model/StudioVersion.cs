using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using Sdl.Community.SdlFreshstart.ViewModel;

namespace Sdl.Community.SdlFreshstart.Model
{
	public class StudioVersion : BaseModel, IStudioVersion
	{
		private const string SdlFolder = @"SDL\SDL Trados Studio";
		private const string SdlBaseRegistryKey = @"HKEY_CURRENT_USER\Software\SDL\SDL Trados Studio\";
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly Dictionary<string, int> _versionToExecutableVersionLegacy = new Dictionary<string, int>
		{
			{"Studio3", 11},
			{"Studio4", 12},
			{"Studio5", 14}
		};

		private bool _isSelected;
		private int _numericVersion;

		public StudioVersion(string versionName, string publicVersion, string edition = "")
		{
			VersionName = $"{versionName}{edition}";
			PublicVersion = publicVersion;
			Edition = edition;
			SdlRegistryKey = $"{SdlBaseRegistryKey}{AppDataStudioFolder}";

			var pluginPath = Path.Combine(SdlFolder, $"{MajorVersion}{edition}");
			var programDataStudioFolderPath = Path.Combine(SdlFolder, ProgramDataStudioFolder);

			ProgramDataStudioPath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
				programDataStudioFolderPath);

			ProgramDataPluginsPath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
				pluginPath);

			var appDataStudioFolderPath = Path.Combine(SdlFolder, AppDataStudioFolder);
			AppDataLocalStudioPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				appDataStudioFolderPath);

			AppDataLocalPluginsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				pluginPath);

			AppDataRoamingStudioPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				appDataStudioFolderPath);

			AppDataRoamingPluginsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				pluginPath);

			ShortVersion = publicVersion.Substring(!publicVersion.ToUpper().Contains("SDL") ? 7 : 11);

			DocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ShortVersion);

			ProjectsXmlPath = Path.Combine(DocumentsPath, "Projects", "projects.xml");

			ProgramDataStudioDataSubfolderPath = $@"{ProgramDataStudioPath}\Updates";

			ProjectTemplatesPath = $@"{DocumentsPath}\Project Templates";

			ProjectApiPath = Directory.GetDirectories(Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"SDL", "ProjectApi")).FirstOrDefault(d => d.Contains(MajorVersion.ToString()));
		}

		public string SdlRegistryKey { get; set; }

		public string AppDataLocalPluginsPath { get; set; }
		public string AppDataLocalStudioPath { get; set; }
		public string AppDataRoamingPluginsPath { get; set; }
		public string AppDataRoamingStudioPath { get; set; }
		public string CacheFolderName => Regex.Replace(PublicVersion, @"\s+", "");
		public string DocumentsPath { get; set; }
		public string Edition { get; set; }

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					OnPropertyChanged(nameof(IsSelected));
				}
			}
		}

		public string LegacyVersion => MajorVersion < 15 ? ExtractNumber(VersionName).ToString() : (string.Empty);

		public int MajorVersion
		{
			get
			{
				if (_numericVersion > 0) return _numericVersion;

				var numericVersion = ExtractNumber(VersionName);
				
				if (numericVersion < 15)
				{
					_logger.Info($"Major Version: Numeric Version {numericVersion}");
					_logger.Info($"Major Version: Version Name {VersionName}");

					_versionToExecutableVersionLegacy.TryGetValue(VersionName, out _numericVersion);
				}
				else
				{
					_numericVersion = numericVersion;
				}

				return _numericVersion;
			}
		}

		public string ProgramDataPluginsPath { get; set; }
		public string ProgramDataStudioDataSubfolderPath { get; set; }
		public string ProgramDataStudioPath { get; set; }
		public string ProjectApiPath { get; set; }
		public string ProjectsXmlPath { get; set; }
		public string ProjectTemplatesPath { get; set; }
		public string PublicVersion { get; set; }
		public string ShortVersion { get; set; }
		public string AppDataStudioFolder => MajorVersion > 15 ? VersionName : $"{MajorVersion}.0.0.0";
		public string ProgramDataStudioFolder => VersionName;
		public string VersionName { get; }
		public string VersionWithEdition => !string.IsNullOrWhiteSpace(Edition) ? $"{ShortVersion} {Edition}" : ShortVersion;

		private int ExtractNumber(string input)
		{
			var regex = new Regex(@"\d+");
			int.TryParse(regex.Match(input).Value, out var numericVersion);
			return numericVersion;
		}
	}
}
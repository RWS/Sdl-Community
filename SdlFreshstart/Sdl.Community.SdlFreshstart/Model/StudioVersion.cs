using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NLog;

namespace Sdl.Community.SdlFreshstart.Model
{
	public class StudioVersion :  IStudioVersion
	{
		private const string SdlBaseRegistryKey = @"HKEY_CURRENT_USER\Software";
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly Dictionary<string, int> _versionToExecutableVersionLegacy = new Dictionary<string, int>
		{
			{"Studio3", 11},
			{"Studio4", 12},
			{"Studio5", 14}
		};

		private bool _isSelected;
		private int _numericVersion;

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public StudioVersion(Versioning.StudioVersion version)
		{
			VersionName = $"{version.Version}{version.Edition}";
			ExecutableVersion = version.ExecutableVersion;
			PublicVersion = version.PublicVersion;
			Edition = version.Edition;
			SdlFolder = ExecutableVersion.Major < 17 ? @"Sdl\Sdl Trados Studio" : @"Trados\Trados Studio";
			SdlRegistryKeys = @$"{SdlFolder}\{AppDataStudioFolder}";

			var pluginPath = Path.Combine(SdlFolder, $"{MajorVersion}{Edition}");
			var programDataStudioFolderPath = Path.Combine(SdlFolder, ProgramDataStudioFolder);

			var srPathPart = ExecutableVersion.Minor > 0 ? $"SR{ExecutableVersion.Minor}" : "";
			var betaPath = version.Edition.ToLower().Equals("beta") ? "_Beta" : "";

			ProgramDataPackagePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Package Cache", ExecutableVersion.Major < 17 ? "SDL" : "Trados", $"{CacheFolderName}{srPathPart}{betaPath}", @"Modules");

			ProgramDataLicenseFolder = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
				programDataStudioFolderPath);

			ProgramDataPluginsFolder = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
				pluginPath);

			var appDataStudioFolderPath = Path.Combine(SdlFolder, AppDataStudioFolder);
			LocalTradosLogsFolder = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				appDataStudioFolderPath);

			LocalPluginsFolder = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				pluginPath);

			GeneralSettingsFolder = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				appDataStudioFolderPath);

			RoamingPluginsFolder = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				pluginPath);

			ShortVersion = version.PublicVersion.Substring(!version.PublicVersion.ToUpper().Contains("SDL") ? 7 : 11);

			DocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				ShortVersion);

			ProjectsXmlPath = Path.Combine(DocumentsPath, "Projects", "projects.xml");

			ProgramDataUpdatesFolder = $@"{ProgramDataLicenseFolder}\Updates";

			ProgramDataProjectTemplatesFolder = $@"{DocumentsPath}\Project Templates";


			_logger.Info("Trying to get the Project API file path...");
			var localProjectApiPath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				SdlFolder);

			_logger.Info($"Local Project APi path: {localProjectApiPath}");
			_logger.Info($"Major version: {MajorVersion}");
			ProjectApiPath = Directory.GetFiles(Path.Combine(localProjectApiPath, AppDataStudioFolder))
				.FirstOrDefault(d => d.Contains("Sdl.ProjectApi.xml"));

			_logger.Info($"Found ProjectAPI Path: {ProjectApiPath}");
		}

		public string SdlRegistryKeys { get; set; }

		private string SdlFolder { get; set; }


		public string LocalPluginsFolder { get; set; }
		public string LocalTradosLogsFolder { get; set; }
		public string RoamingPluginsFolder { get; set; }
		public string GeneralSettingsFolder { get; set; }
		public string CacheFolderName => Regex.Replace(PublicVersion, @"\s+", "");
		public string DocumentsPath { get; set; }
		public string Edition { get; set; }

		public Version ExecutableVersion { get; set; }

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

		public string LegacyVersion => MajorVersion <= 15 ? ExtractNumber(VersionName).ToString() : string.Empty;

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

		public string ProgramDataPluginsFolder { get; set; }
		public string ProgramDataUpdatesFolder { get; set; }
		public string ProgramDataPackagePath { get; }
		public string ProgramDataLicenseFolder { get; set; }
		public string ProjectApiPath { get; set; }
		public string ProjectsXmlPath { get; set; }
		public string ProgramDataProjectTemplatesFolder { get; set; }
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Sdl.Community.SdlFreshstart.Properties;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public class StudioVersion : IStudioVersion, INotifyPropertyChanged
	{
		private const string SdlFolder = @"SDL\SDL Trados Studio";

		private readonly Dictionary<string, int> _versionToExecutableVersionLegacy = new Dictionary<string, int>
		{
			{"Studio3", 11},
			{"Studio4", 12},
			{"Studio5", 14}
		};

		private bool _isSelected;
		private int _numericVersion;

		public string CacheFolderName => Regex.Replace(PublicVersion, @"\s+", "");

		public StudioVersion(string versionName, string publicVersion, string edition = "")
		{
			VersionName = $"{versionName}{edition}";
			PublicVersion = publicVersion;
			Edition = edition;

			var pluginPath = Path.Combine(SdlFolder, $"{ExecutableVersion}{edition}");
			var studioFolderPath = Path.Combine(SdlFolder, StudioFolder);

			ProgramFilesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
				VersionName);
			ProgramDataPaths = new[]
			{
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
					studioFolderPath),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
					pluginPath)
			};
			AppDataLocalPaths = new[]
			{
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
					studioFolderPath),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
					pluginPath)
			};
			AppDataRoamingPaths = new[]
			{
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					studioFolderPath),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					pluginPath)
			};
			DocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				publicVersion.Substring(11));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string[] AppDataLocalPaths { get; set; }
		public string[] AppDataRoamingPaths { get; set; }
		public string DocumentsPath { get; set; }
		public string Edition { get; set; }

		public int ExecutableVersion
		{
			get
			{
				if (_numericVersion > 0) return _numericVersion;

				var numericVersion = ExtractNumber(VersionName);
				_numericVersion = numericVersion < 15 ? _versionToExecutableVersionLegacy[VersionName] : numericVersion;

				return _numericVersion;
			}
		}

		private int ExtractNumber(string input)
		{
			var regex = new Regex(@"\d+");
			int.TryParse(regex.Match(input).Value, out var numericVersion);
			return numericVersion;
		}

		public string LegacyVersion => ExecutableVersion < 15 ? ExtractNumber(VersionName).ToString() : (string.Empty);

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

		public string PluginFolder => $"{ExecutableVersion}{Edition}";
		public string[] ProgramDataPaths { get; set; }
		public string ProgramFilesPath { get; set; }
		public string PublicVersion { get; set; }
		public string StudioFolder => ExecutableVersion > 15 ? VersionName : $"{ExecutableVersion}.0.0.0";
		public string VersionName { get; }

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
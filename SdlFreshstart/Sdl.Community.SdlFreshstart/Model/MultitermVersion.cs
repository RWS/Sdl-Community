using System;
using System.IO;
using System.Text.RegularExpressions;
using Sdl.Community.SdlFreshstart.ViewModel;

namespace Sdl.Community.SdlFreshstart.Model
{
	public class MultitermVersion : BaseModel
	{
		private bool _isSelected;

		public MultitermVersion(string publicVersion,Version executableVersion)
		{
			PublicVersion = publicVersion;
			ExecutableVersion = executableVersion;
		}

		public string CacheFolderName
		{
			get
			{
				var yearOfVersion = Regex.Match(PublicVersion, @"\d+").Value;
				return $"MultiTermDesktop{yearOfVersion}";
			}
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if (_isSelected == value) return;

				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public int MajorVersion => PublicVersion switch
		{
			var x when x.Contains("2017") => 14,
			var x when x.Contains("2019") => 15,
			var x when x.Contains("2021") => 16,
			var x when x.Contains("2022") => 17,
			_ => 0
		};

		public Version ExecutableVersion { get; set; }

		public string MultiTermLocal => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			MultitermFolderPath);

		public string MultiTermRoaming => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			MultitermFolderPath);

		public string MultiTermProgramDataSettings => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
			MultitermFolderPath, "Settings.xml");
		
		public string MultiTermProgramDataUpdates => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
			MultitermFolderPath, "Updates");

		public string PublicVersion { get; }
		public string MultiTermFolder => MajorVersion > 16 ? @"Trados\MultiTerm" : @"SDL\SDL MultiTerm";
		public string MultiTermRegistryKey => $@"{MultiTermFolder}\{VersionName}";
		public string VersionName => $"MultiTerm{MajorVersion}";
		private string MultitermFolderPath => $@"{MultiTermFolder}\{VersionName}";
	}
}
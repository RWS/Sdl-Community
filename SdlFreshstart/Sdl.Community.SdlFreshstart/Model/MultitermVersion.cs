using System;
using System.IO;
using System.Text.RegularExpressions;
using Sdl.Community.SdlFreshstart.ViewModel;

namespace Sdl.Community.SdlFreshstart.Model
{
	public class MultitermVersion : BaseModel
	{
		private bool _isSelected;

		public MultitermVersion(string publicVersion)
		{
			PublicVersion = publicVersion;
		}

		public string CacheFolderName
		{
			get
			{
				var yearOfVersion = Regex.Match(PublicVersion, @"\d+").Value;
				return $"SDLMultiTermDesktop{yearOfVersion}";
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
			_ => 0
		};

		public string MultiTermLocal => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			MultitermFolderPath);

		public string MultiTermRoaming => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			MultitermFolderPath);

		public string MultiTermProgramDataSettings => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
			MultitermFolderPath, "Settings.xml");
		
		public string MultiTermProgramDataUpdates => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
			MultitermFolderPath, "Updates");

		public string PublicVersion { get; }
		public string MultiTermRegistryKey => $@"HKEY_CURRENT_USER\Software\SDL\SDL MultiTerm\{VersionName}";
		public string VersionName => $"MultiTerm{MajorVersion}";
		private string MultitermFolderPath => $@"SDL\SDL MultiTerm\{VersionName}";
	}
}
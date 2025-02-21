using System;
using System.IO;

namespace Trados.Transcreate.Common
{
	public class PathInfo: ICloneable
	{
		private const string SdlCommunityPathName = "Trados AppStore";
		private const string ApplicationPathName = "Transcreate";
		private const string SettingsPathName = "Settings";
		private const string LogsPathName = "Logs";
		private const string IconsPathName = "Icons";
		private const string SettingsFileName = "Settings.xml";
		private const string LanguageMappingsFileName = "LanguageMappings.xlsx";
		private const string ProjectIconFileName = "Transcreate.ico";
		private const string BackTranslationIconFileName = "BackTranslation.ico";

		private string _sdlCommunityFolderPath;
		private string _applicationFolderPath;
		private string _settingsFolderPath;
		private string _logsFolderPath;
		private string _iconsFolderPath;

		public string SdlCommunityFolderPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_sdlCommunityFolderPath))
				{
					return _sdlCommunityFolderPath;
				}

				_sdlCommunityFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					SdlCommunityPathName);

				if (!Directory.Exists(_sdlCommunityFolderPath))
				{
					Directory.CreateDirectory(_sdlCommunityFolderPath);
				}

				return _sdlCommunityFolderPath;
			}
		}

		public string ApplicationFolderPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_applicationFolderPath))
				{
					return _applicationFolderPath;
				}

				_applicationFolderPath = Path.Combine(SdlCommunityFolderPath, ApplicationPathName);
				if (!Directory.Exists(_applicationFolderPath))
				{
					Directory.CreateDirectory(_applicationFolderPath);
				}

				return _applicationFolderPath;
			}
		}

		public string ApplicationLogsFolderPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_logsFolderPath))
				{
					return _logsFolderPath;
				}

				_logsFolderPath = Path.Combine(ApplicationFolderPath, LogsPathName);
				if (!Directory.Exists(_logsFolderPath))
				{
					Directory.CreateDirectory(_logsFolderPath);
				}

				return _logsFolderPath;
			}
		}

		public string ApplicationIconsFolderPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_iconsFolderPath))
				{
					return _iconsFolderPath;
				}

				_iconsFolderPath = Path.Combine(ApplicationFolderPath, IconsPathName);
				if (!Directory.Exists(_iconsFolderPath))
				{
					Directory.CreateDirectory(_iconsFolderPath);
				}

				return _iconsFolderPath;
			}
		}

		public string ProjectIconFilePath => Path.Combine(ApplicationIconsFolderPath, ProjectIconFileName);

		public string BackTranslationIconFilePath => Path.Combine(ApplicationIconsFolderPath, BackTranslationIconFileName);

		public string SettingsFolderPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_settingsFolderPath))
				{
					return _settingsFolderPath;
				}

				_settingsFolderPath = Path.Combine(ApplicationFolderPath, SettingsPathName);
				if (!Directory.Exists(_settingsFolderPath))
				{
					Directory.CreateDirectory(_settingsFolderPath);
				}

				return _settingsFolderPath;
			}
		}

		public string SettingsFilePath => Path.Combine(SettingsFolderPath, SettingsFileName);

		public string LanguageMappingsFilePath => Path.Combine(SettingsFolderPath, LanguageMappingsFileName);
		
		public object Clone()
		{
			return new PathInfo();			
		}
	}
}

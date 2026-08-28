using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ExportAnalysisReports.Service
{
	public class PathInfo
	{
		private const string SdlCommunityPathName = "Trados AppStore";
		private const string ApplicationPathName = "ExportAnalysisReports";
		private const string SettingsPathName = "Settings";
		private const string SettingsFileName = "settings.xml";

		private string _sdlCommunityFullPath;
		private string _applicationFullPath;
		private string _settingsFullPath;

		public string SdlCommunityFullPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_sdlCommunityFullPath))
				{
					return _sdlCommunityFullPath;
				}

				_sdlCommunityFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					SdlCommunityPathName);

				if (!Directory.Exists(_sdlCommunityFullPath))
				{
					Directory.CreateDirectory(_sdlCommunityFullPath);
				}

				return _sdlCommunityFullPath;
			}
		}

		public virtual string ApplicationFullPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_applicationFullPath))
				{
					return _applicationFullPath;
				}

				_applicationFullPath = Path.Combine(SdlCommunityFullPath, ApplicationPathName);
				if (!Directory.Exists(_applicationFullPath))
				{
					Directory.CreateDirectory(_applicationFullPath);
				}

				return _applicationFullPath;
			}
			set
			{
				_applicationFullPath = value;
			}
		}

		public virtual string SettingsFullPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_settingsFullPath))
				{
					return _settingsFullPath;
				}

				_settingsFullPath = Path.Combine(ApplicationFullPath, SettingsPathName);
				if (!Directory.Exists(_settingsFullPath))
				{
					Directory.CreateDirectory(_settingsFullPath);
				}

				return _settingsFullPath;
			}
		}

		public virtual string SettingsFilePath => Path.Combine(SettingsFullPath, SettingsFileName);
	}
}

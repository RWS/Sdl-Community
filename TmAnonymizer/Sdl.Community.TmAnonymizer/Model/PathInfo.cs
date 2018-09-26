using System;
using System.IO;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class PathInfo
	{
		private const string SdlCommunityPathName = "SDL Community";
		private const string TmBackupPathName = "TmAnonymizer Backup";
		private const string ServerTmBackupPathName = "TmAnonymizer ServerBackup";
		private const string SettingsFolderPathName = "TmAnonymizer Settings";
		private const string SettingsFileName = "settings.json";

		private string _sdlCommunityFullPath;
		private string _tmBackupFullPath;
		private string _serverTmBackupFullPath;
		private string _settingsFolderFullPath;

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

		public string TmBackupFullPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_tmBackupFullPath))
				{
					return _tmBackupFullPath;
				}

				_tmBackupFullPath = Path.Combine(SdlCommunityFullPath, TmBackupPathName);
				if (!Directory.Exists(_tmBackupFullPath))
				{
					Directory.CreateDirectory(_tmBackupFullPath);
				}

				return _tmBackupFullPath;
			}
		}

		public string ServerTmBackupFullPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_serverTmBackupFullPath))
				{
					return _serverTmBackupFullPath;
				}

				_serverTmBackupFullPath = Path.Combine(SdlCommunityFullPath, ServerTmBackupPathName);
				if (!Directory.Exists(_serverTmBackupFullPath))
				{
					Directory.CreateDirectory(_serverTmBackupFullPath);
				}

				return _serverTmBackupFullPath;
			}
		}

		public string SettingsFolderFullPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_settingsFolderFullPath))
				{
					return _settingsFolderFullPath;
				}

				_settingsFolderFullPath = Path.Combine(SdlCommunityFullPath, SettingsFolderPathName);
				if (!Directory.Exists(_settingsFolderFullPath))
				{
					Directory.CreateDirectory(_settingsFolderFullPath);
				}

				return _settingsFolderFullPath;
			}
		}

		public string SettingsFilePath => Path.Combine(SettingsFolderFullPath, SettingsFileName);		
	}
}


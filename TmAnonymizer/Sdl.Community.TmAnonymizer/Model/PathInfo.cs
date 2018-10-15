using System;
using System.IO;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class PathInfo
	{
		private const string SdlCommunityPathName = "SDL Community";
		private const string ApplicationPathName = "TmAnonymizer";
		private const string BackupPathName = "Backup";
		private const string TemporaryStoragePathName = "TemporaryStorage";
		private const string SettingsPathName = "Settings";
		private const string SettingsFileName = "settings.json";

		private string _sdlCommunityFullPath;
		private string _applicationFullPath;
		private string _backupFullPath;
		private string _temporaryStorageFullPath;
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

		public string ApplicationFullPath
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
		}

		public string BackupFullPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_backupFullPath))
				{
					return _backupFullPath;
				}

				_backupFullPath = Path.Combine(ApplicationFullPath, BackupPathName);
				if (!Directory.Exists(_backupFullPath))
				{
					Directory.CreateDirectory(_backupFullPath);
				}

				return _backupFullPath;
			}
		}

		public string TemporaryStorageFullPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_temporaryStorageFullPath))
				{
					return _temporaryStorageFullPath;
				}

				_temporaryStorageFullPath = Path.Combine(ApplicationFullPath, TemporaryStoragePathName);
				if (!Directory.Exists(_temporaryStorageFullPath))
				{
					Directory.CreateDirectory(_temporaryStorageFullPath);
				}

				return _temporaryStorageFullPath;
			}
		}

		public string SettingsFullPath
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

		public string SettingsFilePath => Path.Combine(SettingsFullPath, SettingsFileName);		
	}
}


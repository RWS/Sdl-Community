using System;
using System.IO;

namespace IATETerminologyProvider.Model
{
	public class PathInfo
	{
		private const string SdlCommunityPathName = "SDL Community";
		private const string ApplicationPathName = "IATETerminologyProvider";
		private const string TemporaryStoragePathName = "TemporaryStorage";
		private const string SettingsFileName = "IATETerminology.json";
		
		private string _sdlCommunityFullPath;
		private string _applicationFullPath;
		private string _temporaryStorageFullPath;		

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
	
		public string SettingsFilePath => Path.Combine(ApplicationFullPath, SettingsFileName);
	}
}

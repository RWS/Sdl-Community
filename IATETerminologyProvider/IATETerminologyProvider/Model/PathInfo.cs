using System;
using System.IO;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class PathInfo
	{
		private const string RWSAppStorePathName = "Trados AppStore";
		private const string ApplicationPathName = "IATETerminologyProvider";
		private const string TemporaryStoragePathName = "TemporaryStorage";
		private const string DbaseCachePathName = "Cache";
		private const string SettingsFileName = "IATETerminology.json";
		
		private string _rwsAppStoreFullPath;
		private string _applicationFullPath;
		private string _temporaryStorageFullPath;
		private string _dbaseCacheFullPath;

		public string RWSAppStoreFullPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_rwsAppStoreFullPath))
				{
					return _rwsAppStoreFullPath;
				}

				_rwsAppStoreFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					RWSAppStorePathName);

				if (!Directory.Exists(_rwsAppStoreFullPath))
				{
					Directory.CreateDirectory(_rwsAppStoreFullPath);
				}

				return _rwsAppStoreFullPath;
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

				_applicationFullPath = Path.Combine(RWSAppStoreFullPath, ApplicationPathName);
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

		public string DbaseCacheFullPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_dbaseCacheFullPath))
				{
					return _dbaseCacheFullPath;
				}

				_dbaseCacheFullPath = Path.Combine(ApplicationFullPath, DbaseCachePathName);
				if (!Directory.Exists(_dbaseCacheFullPath))
				{
					Directory.CreateDirectory(_dbaseCacheFullPath);
				}

				return _dbaseCacheFullPath;
			}
		}

		public string SettingsFilePath => Path.Combine(ApplicationFullPath, SettingsFileName);
	}
}

using System;
using System.IO;

namespace CustomViewExample.Model
{
	public class CustomViewPathInfo
	{
		private const string AppStorePathName = "Trados AppStore";
		private const string ApplicationPathName = "CustomViewExample";
		private const string SettingsPathName = "Settings";
		private const string SettingsFileName = "Settings.json";
		private const string TipsPathName = "Tips";

		private string _appStoreFullPath;
		private string _applicationFullPath;
		private string _settingsFullPath;
		private string _tipsFullPath;

		public virtual string AppStoreFullPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_appStoreFullPath))
				{
					return _appStoreFullPath;
				}

				_appStoreFullPath = 
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppStorePathName);

				if (!Directory.Exists(_appStoreFullPath))
				{
					Directory.CreateDirectory(_appStoreFullPath);
				}

				return _appStoreFullPath;
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

				_applicationFullPath = Path.Combine(AppStoreFullPath, ApplicationPathName);
				if (!Directory.Exists(_applicationFullPath))
				{
					Directory.CreateDirectory(_applicationFullPath);
				}

				return _applicationFullPath;
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

		public virtual string TipsFullPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_tipsFullPath))
				{
					return _tipsFullPath;
				}

				_tipsFullPath = Path.Combine(ApplicationFullPath, TipsPathName);
				if (!Directory.Exists(_tipsFullPath))
				{
					Directory.CreateDirectory(_tipsFullPath);
				}

				return _tipsFullPath;
			}
		}
	}
}

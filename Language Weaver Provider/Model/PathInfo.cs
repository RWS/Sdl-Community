using LanguageWeaverProvider.Model.Interface;
using System;
using System.IO;

namespace LanguageWeaverProvider.Model
{
    public class PathInfo : IPathInfo
    {
        private const string AppStorePathName = "Trados AppStore";
        private const string ApplicationPathName = "Language Weaver";
        private const string SettingsPathName = "Settings";
        private const string SettingsFileName = "CohereSubscriptionSettings.json";
        private string _appstoreFolderPath;
        private string _applicationFolderPath;
        private string _settingsFolderPath;

        public string AppstoreFolderPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_appstoreFolderPath))
                {
                    return _appstoreFolderPath;
                }

                _appstoreFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    AppStorePathName);

                if (!Directory.Exists(_appstoreFolderPath))
                {
                    Directory.CreateDirectory(_appstoreFolderPath);
                }

                return _appstoreFolderPath;
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

                _applicationFolderPath = Path.Combine(AppstoreFolderPath, ApplicationPathName);
                if (!Directory.Exists(_applicationFolderPath))
                {
                    Directory.CreateDirectory(_applicationFolderPath);
                }

                return _applicationFolderPath;
            }
        }

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

        public string SettingsPath => Path.Combine(SettingsFolderPath, SettingsFileName);
    }

}

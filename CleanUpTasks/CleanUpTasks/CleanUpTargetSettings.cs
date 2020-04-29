using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Sdl.Core.Settings;

namespace SDLCommunityCleanUpTasks
{
	public class CleanUpTargetSettings : SettingsGroup, ICleanUpTargetSettings
    {
        public bool ApplyToNonTranslatables
        {
            get { return GetSetting<bool>(nameof(ApplyToNonTranslatables)); }
            set { GetSetting<bool>(nameof(ApplyToNonTranslatables)).Value = value; }
        }

        public string BackupsSaveFolder
        {
            get { return GetSetting<string>(nameof(BackupsSaveFolder)); }
            set { GetSetting<string>(nameof(BackupsSaveFolder)).Value = value; }
        }

        public Dictionary<string, bool> ConversionFiles
        {
            get { return GetSetting<Dictionary<string, bool>>(nameof(ConversionFiles)); }
            set { GetSetting<Dictionary<string, bool>>(nameof(ConversionFiles)).Value = value; }
        }

        public string LastFileDirectory
        {
            get { return GetSetting<string>(nameof(LastFileDirectory)); }
            set { GetSetting<string>(nameof(LastFileDirectory)).Value = value; }
        }

        public bool MakeBackups
        {
            get { return GetSetting<bool>(nameof(MakeBackups)); }
            set { GetSetting<bool>(nameof(MakeBackups)).Value = value; }
        }

        public bool OverwriteSdlXliff
        {
            get { return GetSetting<bool>(nameof(OverwriteSdlXliff)); }
            set { GetSetting<bool>(nameof(OverwriteSdlXliff)).Value = value; }
        }

        public string SaveFolder
        {
            get { return GetSetting<string>(nameof(SaveFolder)); }
            set { GetSetting<string>(nameof(SaveFolder)).Value = value; }
        }

        public bool SaveTarget
        {
            get { return GetSetting<bool>(nameof(SaveTarget)); }
            set { GetSetting<bool>(nameof(SaveTarget)).Value = value; }
        }

        public ISettingsGroup Settings { get; set; }

        public CultureInfo SourceCulture { get; set; }

        public bool UseConversionSettings
        {
            get { return GetSetting<bool>(nameof(UseConversionSettings)); }
            set { GetSetting<bool>(nameof(UseConversionSettings)).Value = value; }
        }

        [SuppressMessage("Microsoft.Contracts", "TestAlwaysEvaluatingToAConstant")]
        protected override object GetDefaultValue(string settingId)
        {
            switch (settingId)
            {
                case nameof(ApplyToNonTranslatables):
                    return false;

                case nameof(BackupsSaveFolder):
                    return string.Empty;

                case nameof(MakeBackups):
                    return true;

                case nameof(SaveTarget):
                    return true;

                case nameof(OverwriteSdlXliff):
                    return false;

                case nameof(SaveFolder):
                    return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                case nameof(ConversionFiles):
                    return new Dictionary<string, bool>();

                case nameof(LastFileDirectory):
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                case nameof(UseConversionSettings):
                    return true;
            }

            return base.GetDefaultValue(settingId);
        }
    }
}
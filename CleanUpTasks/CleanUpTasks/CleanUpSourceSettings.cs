using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Sdl.Core.Settings;
using SDLCommunityCleanUpTasks.Models;

namespace SDLCommunityCleanUpTasks
{
	public class CleanUpSourceSettings : SettingsGroup, ICleanUpSourceSettings
    {
        #region TagSettingsControl

        public Dictionary<string, bool> FormatTagList
        {
            get { return GetSetting<Dictionary<string, bool>>(nameof(FormatTagList)); }
            set { GetSetting<Dictionary<string, bool>>(nameof(FormatTagList)).Value = value; }
        }

        public Dictionary<string, bool> PlaceholderTagList
        {
            get { return GetSetting<Dictionary<string, bool>>(nameof(PlaceholderTagList)); }
            set { GetSetting<Dictionary<string, bool>>(nameof(PlaceholderTagList)).Value = value; }
        }

        public bool UseTagCleaner
        {
            get { return GetSetting<bool>(nameof(UseTagCleaner)); }
            set { GetSetting<bool>(nameof(UseTagCleaner)).Value = value; }
        }

        #endregion TagSettingsControl

        #region ConversionsSettingsControl

        public bool ApplyToNonTranslatables
        {
            get { return GetSetting<bool>(nameof(ApplyToNonTranslatables)); }
            set { GetSetting<bool>(nameof(ApplyToNonTranslatables)).Value = value; }
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

        public bool UseConversionSettings
        {
            get { return GetSetting<bool>(nameof(UseConversionSettings)); }
            set { GetSetting<bool>(nameof(UseConversionSettings)).Value = value; }
        }

        #endregion ConversionsSettingsControl

        #region SegmentLockerControl

        public BindingList<SegmentLockItem> SegmentLockList
        {
            get { return GetSetting<BindingList<SegmentLockItem>>(nameof(SegmentLockList)); }
            set { GetSetting<BindingList<SegmentLockItem>>(nameof(SegmentLockList)).Value = value; }
        }

        public List<ContextDef> StructureLockList
        {
            get { return GetSetting<List<ContextDef>>(nameof(StructureLockList)); }
            set { GetSetting<List<ContextDef>>(nameof(StructureLockList)).Value = value; }
        }

        public bool UseContentLocker
        {
            get { return GetSetting<bool>(nameof(UseContentLocker)); }
            set { GetSetting<bool>(nameof(UseContentLocker)).Value = value; }
        }

        public bool UseSegmentLocker
        {
            get { return GetSetting<bool>(nameof(UseSegmentLocker)); }
            set { GetSetting<bool>(nameof(UseSegmentLocker)).Value = value; }
        }

        public bool UseStructureLocker
        {
            get { return GetSetting<bool>(nameof(UseStructureLocker)); }
            set { GetSetting<bool>(nameof(UseStructureLocker)).Value = value; }
        }

        #endregion SegmentLockerControl

        public ISettingsGroup Settings { get; set; }

        public CultureInfo SourceCulture { get; set; }

        public List<Placeholder> Placeholders
        {
            get { return GetSetting<List<Placeholder>>(nameof(Placeholders)); }
            set { GetSetting<List<Placeholder>>(nameof(Placeholders)).Value = value; }
        }

        [SuppressMessage("Microsoft.Contracts", "TestAlwaysEvaluatingToAConstant")]
        protected override object GetDefaultValue(string settingId)
        {
            switch (settingId)
            {
                case nameof(UseTagCleaner):
                    return true;

                case nameof(UseConversionSettings):
                    return true;

                case nameof(ApplyToNonTranslatables):
                    return false;

                case nameof(FormatTagList):
                    return new Dictionary<string, bool>()
                    {
                        { TagTable.BackgroundColor, false },
                        { TagTable.Bold, false },
                        { TagTable.FontName, false },
                        { TagTable.FontSize, false },
                        { TagTable.TextColor, false },
                        { TagTable.Italic, false },
                        { TagTable.TextDirection, false },
                        { TagTable.TextPosition, false },
                        { TagTable.Underline, false },
                        { TagTable.Strikethrough, false }
                    };

                case nameof(PlaceholderTagList):
                    return new Dictionary<string, bool>();

                case nameof(ConversionFiles):
                    return new Dictionary<string, bool>();

                case nameof(LastFileDirectory):
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                case nameof(SegmentLockList):
                    return new BindingList<SegmentLockItem>(new List<SegmentLockItem>());

                case nameof(UseSegmentLocker):
                    return true;

                case nameof(UseStructureLocker):
                    return true;

                case nameof(UseContentLocker):
                    return true;

                case nameof(StructureLockList):
                    return new List<ContextDef>();

                case nameof(Placeholders):
                    return new List<Placeholder>();
            }

            return base.GetDefaultValue(settingId);
        }
    }
}
using System.ComponentModel;
using System.Text.RegularExpressions;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.EmbeddedContentProcessor.Settings
{
    public class MatchRule : ISerializableListItem, INotifyPropertyChanged
    {
        const string SettingSegmentionHint = "SegmentationHint";
        const string SettingTagType = "TagType";
        const string SettingStartTagRegex = "StartTagRegex";
        const string SettingEndTagRegex = "EndTagRegex";
        const string SettingIgnoreCase = "IgnoreCase";
        const string SettingContentTranslatable = "ContentTranslatable";
        const string SettingWordStop = "WordStop";
        const string SettingSoftBreak = "SoftBreak";
        const string SettingCanHide = "CanHide";
        const string SettingTextEquivalent = "TextEquivalent";
        const string SettingFormatting = "Formatting";
        const SegmentationHint DefaultSegmentationHint = SegmentationHint.Undefined;
        const TagTypeOption DefaultTagType = TagTypeOption.Placeholder;
        const bool DefaultIgnoreCase = false;
        const bool DefaultContentTranslatable = false;
        const bool DefaultWordStop = false;
        const bool DefaultSoftBreak = false;
        const bool DefaultCanHide = false;


        SegmentationHint _segmentationHint;
        TagTypeOption _tagType;
        string _startTagRegex;
        string _endTagRegex;
        bool _ignoreCase;
        bool _isContentTranslatable;
        bool _isWordStop;
        bool _isSoftBreak;
        bool _canHide;
        string _textEquivalent;
        FormattingGroupSettings _formatting;


        readonly string _defaultStartTagRegex = string.Empty;
        readonly string _defaultEndTagRegex = string.Empty;
        readonly string _defaultTextEquivalent = string.Empty;

        public MatchRule()
        {
            ResetToDefaults();
        }

        public enum TagTypeOption
        {
            Placeholder,
            TagPair
        }

        public SegmentationHint SegmentationHint
        {
            get
            {
                return _segmentationHint;
            }
            set
            {
                _segmentationHint = value;
                OnPropertyChanged("SegmentationHint");
            }
        }

        public TagTypeOption TagType
        {
            get
            {
                return _tagType;
            }
            set
            {
                _tagType = value;
                OnPropertyChanged("TagType");
            }
        }

        public string StartTagRegexValue
        {
            get
            {
                return _startTagRegex;
            }
            set
            {
                _startTagRegex = value;
                OnPropertyChanged("StartTagRegexValue");

            }
        }

        public string EndTagRegexValue
        {
            get
            {
                return _endTagRegex;
            }
            set
            {
                _endTagRegex = value;
                OnPropertyChanged("EndTagRegexValue");
            }
        }

        public bool IgnoreCase
        {
            get
            {
                return _ignoreCase;
            }
            set
            {
                _ignoreCase = value;
                OnPropertyChanged("IgnoreCase");
            }
        }

        public bool IsContentTranslatable
        {
            get
            {
                return _isContentTranslatable;
            }
            set
            {
                _isContentTranslatable = value;
                OnPropertyChanged("IsContentTranslatable");
            }
        }

        public bool IsWordStop
        {
            get
            {
                return _isWordStop;
            }
            set
            {
                _isWordStop = value;
                OnPropertyChanged("IsWordStop");
            }
        }

        public bool IsSoftBreak
        {
            get
            {
                return _isSoftBreak;
            }
            set
            {
                _isSoftBreak = value;
                OnPropertyChanged("IsSoftBreak");
            }
        }

        public bool CanHide
        {
            get
            {
                return _canHide;
            }
            set
            {
                _canHide = value;
                OnPropertyChanged("CanHide");
            }
        }

        public string TextEquivalent
        {
            get
            {
                return _textEquivalent;
            }
            set
            {
                _textEquivalent = value;
                OnPropertyChanged("TextEquivalent");
            }
        }

        public FormattingGroupSettings Formatting
        {
            get
            {
                return _formatting;
            }
            set
            {
                _formatting = value;
                OnPropertyChanged("Formatting");
            }
        }

        public Regex BuildStartTagRegex()
        {
            return new Regex(StartTagRegexValue,
                IgnoreCase
                ? RegexOptions.IgnoreCase
                : RegexOptions.None);
        }

        public Regex BuildEndTagRegex()
        {
            return new Regex(EndTagRegexValue,
                IgnoreCase
                ? RegexOptions.IgnoreCase
                : RegexOptions.None);
        }


        public void ResetToDefaults()
        {
            SegmentationHint = DefaultSegmentationHint;
            TagType = DefaultTagType;
            StartTagRegexValue = _defaultStartTagRegex;
            EndTagRegexValue = _defaultEndTagRegex;
            IgnoreCase = DefaultIgnoreCase;
            IsContentTranslatable = DefaultContentTranslatable;
            IsWordStop = DefaultWordStop;
            IsSoftBreak = DefaultSoftBreak;
            CanHide = DefaultCanHide;
            TextEquivalent = _defaultTextEquivalent;
            Formatting = new FormattingGroupSettings();
        }

        #region ISerializableListItem Members

        public void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting)
        {
            settingsGroup.RemoveSetting(listItemSetting + SettingSegmentionHint);
            settingsGroup.RemoveSetting(listItemSetting + SettingTagType);
            settingsGroup.RemoveSetting(listItemSetting + SettingStartTagRegex);
            settingsGroup.RemoveSetting(listItemSetting + SettingEndTagRegex);
            settingsGroup.RemoveSetting(listItemSetting + SettingIgnoreCase);
            settingsGroup.RemoveSetting(listItemSetting + SettingContentTranslatable);
            settingsGroup.RemoveSetting(listItemSetting + SettingWordStop);
            settingsGroup.RemoveSetting(listItemSetting + SettingSoftBreak);
            settingsGroup.RemoveSetting(listItemSetting + SettingCanHide);
            settingsGroup.RemoveSetting(listItemSetting + SettingTextEquivalent);
            var formattingSettings = new FormattingGroupSettings();
            formattingSettings.ClearListItemSettings(settingsGroup, listItemSetting + SettingFormatting);
        }

        public void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
        {
            SegmentationHint = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingSegmentionHint, DefaultSegmentationHint);
            TagType = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingTagType, DefaultTagType);
            StartTagRegexValue = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingStartTagRegex, _defaultStartTagRegex);
            EndTagRegexValue = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingEndTagRegex, _defaultEndTagRegex);
            IgnoreCase = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingIgnoreCase, DefaultIgnoreCase);
            IsContentTranslatable = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingContentTranslatable, DefaultContentTranslatable);
            IsWordStop = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingWordStop, DefaultWordStop);
            IsSoftBreak = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingSoftBreak, DefaultSoftBreak);
            CanHide = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingCanHide, DefaultCanHide);
            TextEquivalent = GetSettingFromSettingsGroup(settingsGroup, listItemSetting + SettingTextEquivalent, _defaultTextEquivalent);
            if (settingsGroup.ContainsSetting(listItemSetting + SettingFormatting))
            {
                _formatting = new FormattingGroupSettings();
                _formatting.PopulateFromSettingsGroup(settingsGroup, listItemSetting + SettingFormatting);
            }
        }

        public void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
        {
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingSegmentionHint, SegmentationHint, DefaultSegmentationHint);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingTagType, TagType, DefaultTagType);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingStartTagRegex, StartTagRegexValue, _defaultStartTagRegex);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingEndTagRegex, EndTagRegexValue, _defaultEndTagRegex);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingIgnoreCase, IgnoreCase, DefaultIgnoreCase);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingContentTranslatable, IsContentTranslatable, DefaultContentTranslatable);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingWordStop, IsWordStop, DefaultWordStop);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingSoftBreak, IsSoftBreak, DefaultSoftBreak);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingCanHide, CanHide, DefaultCanHide);
            UpdateSettingInSettingsGroup(settingsGroup, listItemSetting + SettingTextEquivalent, TextEquivalent, _defaultTextEquivalent);
            if (Formatting != null && Formatting.FormattingItems.Count > 0)
            {
                settingsGroup.GetSetting<bool>(listItemSetting + SettingFormatting).Value = true;

                _formatting.SaveToSettingsGroup(settingsGroup, listItemSetting + SettingFormatting);

            }

        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        protected T GetSettingFromSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T defaultValue)
        {
            if (settingsGroup.ContainsSetting(settingName))
            {
                return settingsGroup.GetSetting<T>(settingName).Value;
            }

            return defaultValue;
        }


        protected void UpdateSettingInSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T settingValue, T defaultValue)
        {
            if (settingsGroup == null)
            {
                return;
            }
            if (settingsGroup.ContainsSetting(settingName))
            {
                settingsGroup.GetSetting<T>(settingName).Value = settingValue;
            }
            else
            {
                if (settingValue == null)
                {
                    if ((defaultValue != null))
                    {
                        settingsGroup.GetSetting<T>(settingName).Value = default(T);
                    }
                }
                else
                    if (!settingValue.Equals(defaultValue))
                    {
                        settingsGroup.GetSetting<T>(settingName).Value = settingValue;
                    }
            }

        }
    }
}

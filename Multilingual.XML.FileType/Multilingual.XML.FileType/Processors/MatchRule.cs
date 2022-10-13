using System;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Processors
{
    public class MatchRule : AbstractSettingsClass
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

        SegmentationHint DefaultSegmentationHint = SegmentationHint.MayExclude;
        TagTypeOption DefaultTagType = TagTypeOption.Placeholder;
        string DefaultStartTagRegex = string.Empty;
        string DefaultEndTagRegex = string.Empty;
        bool DefaultIgnoreCase = false;
        bool DefaultContentTranslatable = false;
        bool DefaultWordStop = false;
        bool DefaultSoftBreak = false;
        bool DefaultCanHide = false;
        string DefaultTextEquivalent = string.Empty;

        public MatchRule()
        {
            ResetToDefaults();
        }

        public enum TagTypeOption
        {
            Placeholder,
            TagPair
        }

        public override string SettingName => "MatchRule";

        public SegmentationHint SegmentationHint { get; set; }
        public TagTypeOption TagType { get; set; }

        public string StartTagRegexValue { get; set; }

        public string EndTagRegexValue { get; set; }

        public bool IgnoreCase { get; set; }

        public bool IsContentTranslatable { get; set; }

        public bool IsWordStop { get; set; }
        public bool IsSoftBreak { get; set; }

        public bool CanHide { get; set; }

        public string TextEquivalent { get; set; }

        public FormattingGroupSettings Formatting { get; set; }

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


        public override void Read(IValueGetter valueGetter)
        {
            SegmentationHint = (SegmentationHint)Enum.Parse(typeof(SegmentationHint), valueGetter.GetValue(SettingSegmentionHint, DefaultSegmentationHint.ToString("G")));
            TagType = (TagTypeOption)Enum.Parse(typeof(TagTypeOption), valueGetter.GetValue(SettingTagType, DefaultTagType.ToString("G")));

            StartTagRegexValue = valueGetter.GetValue(SettingStartTagRegex, DefaultStartTagRegex);
            EndTagRegexValue = valueGetter.GetValue(SettingEndTagRegex, DefaultEndTagRegex);
            IgnoreCase = valueGetter.GetValue(SettingIgnoreCase, DefaultIgnoreCase);
            IsContentTranslatable = valueGetter.GetValue(SettingContentTranslatable, DefaultContentTranslatable);
            IsWordStop = valueGetter.GetValue(SettingWordStop, DefaultWordStop);
            IsSoftBreak = valueGetter.GetValue(SettingSoftBreak, DefaultSoftBreak);
            CanHide = valueGetter.GetValue(SettingCanHide, DefaultCanHide);
            TextEquivalent = valueGetter.GetValue(SettingTextEquivalent, DefaultTextEquivalent);
            Formatting = valueGetter.GetValue(SettingFormatting, new FormattingGroupSettings(), false);
        }

        public override void Save(IValueProcessor valueProcessor)
        {
            valueProcessor.Process(SettingSegmentionHint, SegmentationHint.ToString("G"), DefaultSegmentationHint.ToString("G"));
            valueProcessor.Process(SettingTagType, TagType.ToString("G"), DefaultTagType.ToString("G"));
            valueProcessor.Process(SettingStartTagRegex, StartTagRegexValue, DefaultStartTagRegex);
            valueProcessor.Process(SettingEndTagRegex, EndTagRegexValue, DefaultEndTagRegex);
            valueProcessor.Process(SettingIgnoreCase, IgnoreCase, DefaultIgnoreCase);
            valueProcessor.Process(SettingContentTranslatable, IsContentTranslatable, DefaultContentTranslatable);
            valueProcessor.Process(SettingWordStop, IsWordStop, DefaultWordStop);
            valueProcessor.Process(SettingSoftBreak, IsSoftBreak, DefaultSoftBreak);
            valueProcessor.Process(SettingCanHide, CanHide, DefaultCanHide);
            valueProcessor.Process(SettingTextEquivalent, TextEquivalent, DefaultTextEquivalent);

            if (Formatting != null && Formatting.FormattingItems.Count > 0)
            {
                valueProcessor.Process(SettingFormatting, Formatting, new FormattingGroupSettings(), false);
            }
        }

        public override object Clone()
        {
            return new MatchRule
            {
                SegmentationHint = SegmentationHint,
                TagType = TagType,
                StartTagRegexValue = StartTagRegexValue,
                EndTagRegexValue = EndTagRegexValue,
                IgnoreCase = IgnoreCase,
                IsContentTranslatable = IsContentTranslatable,
                IsWordStop = IsWordStop,
                IsSoftBreak = IsSoftBreak,
                CanHide = CanHide,
                TextEquivalent = TextEquivalent,
                Formatting = Formatting
            };
        }

        public override bool Equals(ISettingsClass other)
        {
            return other is MatchRule otherMatchRule &&
                   otherMatchRule.SegmentationHint == SegmentationHint &&
                   otherMatchRule.TagType == TagType &&
                   otherMatchRule.StartTagRegexValue == StartTagRegexValue &&
                   otherMatchRule.EndTagRegexValue == EndTagRegexValue &&
                   otherMatchRule.IgnoreCase == IgnoreCase &&
                   otherMatchRule.IsContentTranslatable == IsContentTranslatable &&
                   otherMatchRule.IsWordStop == IsWordStop &&
                   otherMatchRule.IsSoftBreak == IsSoftBreak &&
                   otherMatchRule.CanHide == CanHide &&
                   otherMatchRule.TextEquivalent == TextEquivalent &&
                   otherMatchRule.Formatting.Equals(Formatting);
        }


        public override void ResetToDefaults()
        {
            SegmentationHint = DefaultSegmentationHint;
            TagType = DefaultTagType;
            StartTagRegexValue = DefaultStartTagRegex;
            EndTagRegexValue = DefaultEndTagRegex;
            IgnoreCase = DefaultIgnoreCase;
            IsContentTranslatable = DefaultContentTranslatable;
            IsWordStop = DefaultWordStop;
            IsSoftBreak = DefaultSoftBreak;
            CanHide = DefaultCanHide;
            TextEquivalent = DefaultTextEquivalent;
            Formatting = new FormattingGroupSettings();
        }

    }
}

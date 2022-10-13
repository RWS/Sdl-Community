using System.Collections.Generic;
using System.Linq;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Multilingual.XML.FileType.Processors
{
    public class EmbeddedContentRegexSettings : AbstractSettingsClass
    {
        private const string SettingRegexEmbeddingEnabled = "RegexEmbeddingEnabled";
        private const string SettingStructureInfoList = "StructureInfoList";
        private const string SettingMatchRuleList = "MatchRuleList";
        private const string SettingFilterByStructureInfo = "FilterByStructureInfo";
        public override string SettingName => "EmbeddedContentRegexSettings";

        protected virtual bool DefaultRegexEmbeddingEnabled => false;
        protected virtual bool DefaultFilterByStructureInfo => true;
        protected virtual List<MatchRule> DefaultMatchRules => new List<MatchRule>();
        protected virtual List<string> DefaultStructureInfos => new List<string>();

        public bool Enabled { get; set; }
        public bool FilterByStructureInfo { get; set; }
        public List<string> StructureInfos { get; set; }
        public List<MatchRule> MatchRules { get; set; }

        public EmbeddedContentRegexSettings()
        {
	        ResetToDefaults();
        }

        public override bool Equals(ISettingsClass other)
        {
            return other is EmbeddedContentRegexSettings otherSettings &&
                   otherSettings.Enabled == Enabled &&
                   otherSettings.FilterByStructureInfo == FilterByStructureInfo &&
                   otherSettings.StructureInfos.SequenceEqual(StructureInfos) &&
                   otherSettings.MatchRules.SequenceEqual(MatchRules);
        }

        public override void Read(IValueGetter valueGetter)
        {
            Enabled = valueGetter.GetValue(SettingRegexEmbeddingEnabled, DefaultRegexEmbeddingEnabled);
            FilterByStructureInfo = valueGetter.GetValue(SettingFilterByStructureInfo, DefaultFilterByStructureInfo);
            StructureInfos = valueGetter.GetStringList(SettingStructureInfoList, StructureInfos);
            MatchRules = valueGetter.GetCompositeList(SettingMatchRuleList, MatchRules);
        }

        public override void Save(IValueProcessor valueProcessor)
        {
            valueProcessor.Process(SettingRegexEmbeddingEnabled, Enabled, DefaultRegexEmbeddingEnabled);
            valueProcessor.Process(SettingFilterByStructureInfo, FilterByStructureInfo, DefaultFilterByStructureInfo);
            valueProcessor.Process(SettingStructureInfoList, StructureInfos, DefaultStructureInfos);
            valueProcessor.Process(SettingMatchRuleList, MatchRules, DefaultMatchRules);
        }

        /// <summary>
        /// Returns a deep copy of the current instance
        /// </summary>
        /// <remarks>
        ///     Note: All derived classes should override this method! The object returned has to
        ///     be of the derived type to make sure the correct override of ResetToDefaults is called.
        /// </remarks>
        public override object Clone()
        {
            return new EmbeddedContentRegexSettings
            {
                Enabled = Enabled,
                FilterByStructureInfo = FilterByStructureInfo,
                StructureInfos = new List<string>(StructureInfos),
                MatchRules = MatchRules.Select(r => r.Clone() as MatchRule).ToList()
            };
        }

        public override void ResetToDefaults()
        {
            Enabled = DefaultRegexEmbeddingEnabled;
            FilterByStructureInfo = DefaultFilterByStructureInfo;
            StructureInfos = DefaultStructureInfos;
            MatchRules = DefaultMatchRules;
        }
    }
}

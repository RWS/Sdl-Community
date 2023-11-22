using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Multilingual.XML.FileType.Settings
{
	public class EmbeddedContentProcessingSettings : AbstractSettingsClass
    {
        private const bool DefaultProcessEmbeddedContentInsideElements = false;
        private const bool DefaultProcessEmbeddedContentInsideCdata = false;
        protected virtual bool DefaultProcessDocStructInfoEmbeddedContent => false;
        protected virtual bool DefaultProcessEmbeddedContent => false;
        private const string DefaultCdataProcessorId = "";

        private const string SettingProcessEmbeddedContentInsideElements = "ProcessEmbeddedContentInsideElements";
        private const string SettingProcessEmbeddedContent = "ProcessEmbeddedContent";
        private const string SettingEmbeddedProcessorRules = "EmbeddedProcessorRules_";
        private const string SettingProcessEmbeddedContentInsideCdata = "ProcessEmbeddedContentInsideCdata";
        private const string SettingProcessDocStructInfoEmbeddedContent = "ProcessDocStructInfoEmbeddedContent";
        private const string SettingCdataProcessorId = "CdataProcessorId";
        public override string SettingName => "EmbeddedContentProcessingSettings";

        public bool ProcessEmbeddedContent { get; set; }

        public bool ProcessEmbeddedContentInsideElements { get; set; }

        public bool ProcessEmbeddedContentInsideCdata { get; set; }

        public bool ProcessDocStructInfoEmbeddedContent { get; set; }

        public List<EmbeddedProcessorRule> EmbeddedProcessorRules { get; set; }

        public string CdataProcessorId { get; set; }

        public EmbeddedContentProcessingSettings()
        {
            ResetToDefaults();
        }

        public override bool Equals(ISettingsClass other)
        {
            return other is EmbeddedContentProcessingSettings otherSetting &&
                   otherSetting.ProcessEmbeddedContent == ProcessEmbeddedContent &&
                   otherSetting.ProcessEmbeddedContentInsideElements == ProcessEmbeddedContentInsideElements &&
                   otherSetting.ProcessEmbeddedContentInsideCdata == ProcessEmbeddedContentInsideCdata &&
                   otherSetting.ProcessDocStructInfoEmbeddedContent == ProcessDocStructInfoEmbeddedContent &&
                   otherSetting.CdataProcessorId == CdataProcessorId &&
                   otherSetting.EmbeddedProcessorRules.SequenceEqual(EmbeddedProcessorRules);
        }

        public override void Read(IValueGetter valueGetter)
        {
            ProcessEmbeddedContent = valueGetter.GetValue(SettingProcessEmbeddedContent,
                DefaultProcessEmbeddedContent);

            ProcessEmbeddedContentInsideElements = valueGetter.GetValue(
                SettingProcessEmbeddedContentInsideElements, DefaultProcessEmbeddedContentInsideElements);
            EmbeddedProcessorRules = valueGetter.GetCompositeList(SettingEmbeddedProcessorRules, GetDefaultEmbeddedProcessorRules());

            ProcessEmbeddedContentInsideCdata = valueGetter.GetValue(
                SettingProcessEmbeddedContentInsideCdata, DefaultProcessEmbeddedContentInsideCdata);

            ProcessDocStructInfoEmbeddedContent = valueGetter.GetValue(
                SettingProcessDocStructInfoEmbeddedContent, DefaultProcessDocStructInfoEmbeddedContent);

            CdataProcessorId = valueGetter.GetValue(SettingCdataProcessorId, DefaultCdataProcessorId);
        }

        public override void Save(IValueProcessor valueProcessor)
        {
            valueProcessor.Process(SettingProcessEmbeddedContent, ProcessEmbeddedContent,
                 DefaultProcessEmbeddedContent);

            valueProcessor.Process(SettingProcessDocStructInfoEmbeddedContent, ProcessDocStructInfoEmbeddedContent,
                DefaultProcessDocStructInfoEmbeddedContent);

            valueProcessor.Process(SettingProcessEmbeddedContentInsideElements,
                ProcessEmbeddedContentInsideElements, DefaultProcessEmbeddedContentInsideElements);
            valueProcessor.Process(SettingEmbeddedProcessorRules, EmbeddedProcessorRules, GetDefaultEmbeddedProcessorRules());

            valueProcessor.Process(SettingProcessEmbeddedContentInsideCdata, ProcessEmbeddedContentInsideCdata,
                DefaultProcessEmbeddedContentInsideCdata);
            valueProcessor.Process(SettingCdataProcessorId, CdataProcessorId, DefaultCdataProcessorId);
        }

        public override object Clone()
        {
            var clone = GetDefaultEmbeddedContentProcessingSettings();

            clone.ProcessEmbeddedContent = ProcessEmbeddedContent;
            clone.ProcessEmbeddedContentInsideElements = ProcessEmbeddedContentInsideElements;
            clone.ProcessEmbeddedContentInsideCdata = ProcessEmbeddedContentInsideCdata;
            clone.ProcessDocStructInfoEmbeddedContent = ProcessDocStructInfoEmbeddedContent;
            clone.CdataProcessorId = CdataProcessorId;
            clone.EmbeddedProcessorRules =
                EmbeddedProcessorRules.Select(r => r.Clone() as EmbeddedProcessorRule).ToList();

            return clone;
        }

        public override void ResetToDefaults()
        {
            ProcessEmbeddedContent = DefaultProcessEmbeddedContent;

            ProcessEmbeddedContentInsideElements = DefaultProcessEmbeddedContentInsideElements;
            EmbeddedProcessorRules = GetDefaultEmbeddedProcessorRules();

            ProcessEmbeddedContentInsideCdata = DefaultProcessEmbeddedContentInsideCdata;
            CdataProcessorId = DefaultCdataProcessorId;

            ProcessDocStructInfoEmbeddedContent = DefaultProcessDocStructInfoEmbeddedContent;
        }

        private List<EmbeddedProcessorRule> GetDefaultEmbeddedProcessorRules()
        {
            return new List<EmbeddedProcessorRule>();
        }

        protected virtual EmbeddedContentProcessingSettings GetDefaultEmbeddedContentProcessingSettings()
        {
            return new EmbeddedContentProcessingSettings();
        }

        protected override bool HasEmbeddedContentProcessorIds => true;

        protected override IEnumerable<string> GetEmbeddedContentProcessorIds()
        {
            if (!ProcessEmbeddedContent)
                yield break;

            if (ProcessEmbeddedContentInsideCdata && !string.IsNullOrEmpty(CdataProcessorId))
                yield return CdataProcessorId;

            if (ProcessEmbeddedContentInsideElements)
            {
                foreach (var id in EmbeddedProcessorRules.Select(x => x.EmbeddedProcessorId))
                    if (!string.IsNullOrEmpty(id))
                        yield return id;
            }
        }
    }
}

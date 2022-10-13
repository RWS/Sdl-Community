using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Multilingual.XML.FileType.Settings
{
	public class EmbeddedProcessorRule : AbstractSettingsClass
	{
		private const string SettingParserRuleXPathSelector = "ParserRuleXPathSelector";
		private const string SettingEmbeddedProcessorId = "EmbeddedProcessorId";
		public override string SettingName => "EmbeddedProcessorRule";

		public string ParserRuleXPathSelector { get; set; }

		public string EmbeddedProcessorId { get; set; }

		public EmbeddedProcessorRule()
		{
			ResetToDefaults();
		}


		public override void Read(IValueGetter valueGetter)
		{
			ParserRuleXPathSelector = valueGetter.GetValue(SettingParserRuleXPathSelector, string.Empty);
			EmbeddedProcessorId = valueGetter.GetValue(SettingEmbeddedProcessorId, string.Empty);
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process(SettingParserRuleXPathSelector, ParserRuleXPathSelector, string.Empty);
			valueProcessor.Process(SettingEmbeddedProcessorId, EmbeddedProcessorId, string.Empty);
		}

		public override object Clone()
		{
			return new EmbeddedProcessorRule
			{
				EmbeddedProcessorId = EmbeddedProcessorId,
				ParserRuleXPathSelector = SettingParserRuleXPathSelector
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			return other is EmbeddedProcessorRule otherSetting &&
			       otherSetting.ParserRuleXPathSelector == ParserRuleXPathSelector &&
			       otherSetting.EmbeddedProcessorId == EmbeddedProcessorId;
		}

		public override void ResetToDefaults()
		{
			ParserRuleXPathSelector = string.Empty;
			EmbeddedProcessorId = string.Empty;
		}
	}
}

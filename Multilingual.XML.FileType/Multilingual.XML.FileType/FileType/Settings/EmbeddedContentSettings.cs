using System;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Multilingual.XML.FileType.FileType.Settings
{
	public class EmbeddedContentSettings : AbstractSettingsClass
	{
		private const string EmbeddedContentProcessSetting = "EmbeddedContentProcess";
		private const string EmbeddedContentProcessorIdSetting = "EmbeddedContentProcessorId";
		private const string EmbeddedContentFoundInSetting = "EmbeddedContentFoundIn";
		
		public enum FoundIn
		{
			All,
			CDATA
		}

		private readonly bool _embeddedContentProcessDefault;
		private readonly string _embeddedContentProcessorIdDefault;
		private readonly FoundIn _embeddedContentFoundInDefault;

		public EmbeddedContentSettings()
		{
			_embeddedContentProcessDefault = false;
			_embeddedContentProcessorIdDefault = string.Empty;
			_embeddedContentFoundInDefault = FoundIn.CDATA;

			ResetToDefaults();
		}

		public bool EmbeddedContentProcess { get; set; }

		public string EmbeddedContentProcessorId { get; set; }

		public FoundIn EmbeddedContentFoundIn { get; set; }


		public override string SettingName => "MultilingualXmlEmbeddedContentSettings";

		public override void Read(IValueGetter valueGetter)
		{
			EmbeddedContentProcess = valueGetter.GetValue(EmbeddedContentProcessSetting, _embeddedContentProcessDefault);
			EmbeddedContentProcessorId = valueGetter.GetValue(EmbeddedContentProcessorIdSetting, _embeddedContentProcessorIdDefault);
			
			var embeddedContentFoundIn = valueGetter.GetValue(EmbeddedContentFoundInSetting, _embeddedContentFoundInDefault.ToString());
			var success = Enum.TryParse<FoundIn>(embeddedContentFoundIn, out var foundInResult);
			EmbeddedContentFoundIn = success ? foundInResult : FoundIn.CDATA;
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process(EmbeddedContentProcessSetting, EmbeddedContentProcess, _embeddedContentProcessDefault);
			valueProcessor.Process(EmbeddedContentProcessorIdSetting, EmbeddedContentProcessorId, _embeddedContentProcessorIdDefault);
			valueProcessor.Process(EmbeddedContentFoundInSetting, EmbeddedContentFoundIn.ToString(), _embeddedContentFoundInDefault.ToString());
		}

		public override object Clone()
		{
			return new EmbeddedContentSettings
			{
				EmbeddedContentProcess = EmbeddedContentProcess,
				EmbeddedContentProcessorId = EmbeddedContentProcessorId,
				EmbeddedContentFoundIn = EmbeddedContentFoundIn
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			return other is EmbeddedContentSettings otherSetting &&
				   otherSetting.EmbeddedContentProcess == EmbeddedContentProcess &&
				   otherSetting.EmbeddedContentProcessorId == EmbeddedContentProcessorId &&
				   otherSetting.EmbeddedContentFoundIn == EmbeddedContentFoundIn;
		}

		public sealed override void ResetToDefaults()
		{
			EmbeddedContentProcess = _embeddedContentProcessDefault;
			EmbeddedContentProcessorId = _embeddedContentProcessorIdDefault;

			var success = Enum.TryParse<FoundIn>(_embeddedContentFoundInDefault.ToString(), out var foundInResult);
			EmbeddedContentFoundIn = success ? foundInResult : FoundIn.CDATA;
		}
	}
}

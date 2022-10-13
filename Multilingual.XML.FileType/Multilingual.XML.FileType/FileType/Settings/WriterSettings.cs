using System;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Multilingual.XML.FileType.FileType.Settings
{
	public class WriterSettings : AbstractSettingsClass
	{
		private const string LanguageMappingMonoLanguageSetting = "LanguageMappingMonoLanguage";
		private readonly bool _languageMappingMonoLanguageDefault;

		public WriterSettings()
		{
			_languageMappingMonoLanguageDefault = false;

			ResetToDefaults();
		}

		public bool LanguageMappingMonoLanguage { get; set; }


		public override string SettingName => "MultilingualXmlWriterSettings";

		public override void Read(IValueGetter valueGetter)
		{
			LanguageMappingMonoLanguage = valueGetter.GetValue(LanguageMappingMonoLanguageSetting, _languageMappingMonoLanguageDefault);
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process(LanguageMappingMonoLanguageSetting, LanguageMappingMonoLanguage, _languageMappingMonoLanguageDefault);
		}

		public override object Clone()
		{
			return new WriterSettings
			{
				LanguageMappingMonoLanguage = LanguageMappingMonoLanguage
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			return other is WriterSettings otherSetting &&
			       otherSetting.LanguageMappingMonoLanguage == LanguageMappingMonoLanguage;
		}

		public sealed override void ResetToDefaults()
		{
			LanguageMappingMonoLanguage = _languageMappingMonoLanguageDefault;
		}
	}
}

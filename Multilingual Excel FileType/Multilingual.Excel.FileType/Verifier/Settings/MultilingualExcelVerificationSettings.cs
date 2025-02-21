using Sdl.Core.Settings;

namespace Multilingual.Excel.FileType.Verifier.Settings
{
	public class MultilingualExcelVerificationSettings : SettingsGroup
	{
		private const string MaxCharacterLengthEnabled_Setting = "MaxCharacterLengthEnabled";
		private const string MaxCharacterLengthSeverity_Setting = "MaxCharacterLengthSeverity";

		private const string MaxPixelLengthEnabled_Setting = "MaxPixelLengthEnabled";
		private const string MaxPixelLengthSeverity_Setting = "MaxPixelLengthSeverity";

		private const string MaxLinesPerParagraphEnabled_Setting = "MaxLinesPerParagraphEnabled";
		private const string MaxLinesPerParagraphSeverity_Setting = "MaxLinesPerParagraphSeverity";

		private const string VerifySourceParagraphs_Setting = "VerifySourceParagraphs";
		private const string VerifyTargetParagraphs_Setting = "VerifyTargetParagraphs";

		public Setting<bool> MaxCharacterLengthEnabled
		{
			get => GetSetting<bool>(MaxCharacterLengthEnabled_Setting);
			set => GetSetting<bool>(MaxCharacterLengthEnabled_Setting).Value = value;
		}

		public Setting<int> MaxCharacterLengthSeverity
		{
			get => GetSetting<int>(MaxCharacterLengthSeverity_Setting);
			set => GetSetting<int>(MaxCharacterLengthSeverity_Setting).Value = value;
		}

		public Setting<bool> MaxPixelLengthEnabled
		{
			get => GetSetting<bool>(MaxPixelLengthEnabled_Setting);
			set => GetSetting<bool>(MaxPixelLengthEnabled_Setting).Value = value;
		}

		public Setting<int> MaxPixelLengthSeverity
		{
			get => GetSetting<int>(MaxPixelLengthSeverity_Setting);
			set => GetSetting<int>(MaxPixelLengthSeverity_Setting).Value = value;
		}

		public Setting<bool> MaxLinesPerParagraphEnabled
		{
			get => GetSetting<bool>(MaxLinesPerParagraphEnabled_Setting);
			set => GetSetting<bool>(MaxLinesPerParagraphEnabled_Setting).Value = value;
		}

		public Setting<int> MaxLinesPerParagraphSeverity
		{
			get => GetSetting<int>(MaxLinesPerParagraphSeverity_Setting);
			set => GetSetting<int>(MaxLinesPerParagraphSeverity_Setting).Value = value;
		}

		public Setting<bool> VerifySourceParagraphsEnabled
		{
			get => GetSetting<bool>(VerifySourceParagraphs_Setting);
			set => GetSetting<bool>(VerifySourceParagraphs_Setting).Value = value;
		}

		public Setting<bool> VerifyTargetParagraphsEnabled
		{
			get => GetSetting<bool>(VerifyTargetParagraphs_Setting);
			set => GetSetting<bool>(VerifyTargetParagraphs_Setting).Value = value;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case MaxCharacterLengthEnabled_Setting:
					return true;
				case MaxCharacterLengthSeverity_Setting:
					return 1;
				case MaxPixelLengthEnabled_Setting:
					return true;
				case MaxPixelLengthSeverity_Setting:
					return 1;
				case MaxLinesPerParagraphEnabled_Setting:
					return false;
				case MaxLinesPerParagraphSeverity_Setting:
					return 1;
				case VerifySourceParagraphs_Setting:
					return false;
				case VerifyTargetParagraphs_Setting:
					return true;
				default:
					return base.GetDefaultValue(settingId);
			}
		}
	}
}

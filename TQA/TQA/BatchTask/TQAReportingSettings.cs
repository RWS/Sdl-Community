using Sdl.Core.Settings;


namespace Sdl.Community.TQA.BatchTask
{
	public class TQAReportingSettings : SettingsGroup
	{
		private const string TQAReportingQualityId = "TQAReportingQuality";
		private const string TQAReportingDefaultQuality = "Premium";


		public string TQAReportingQuality
		{
			get => GetSetting<string>(TQAReportingQualityId, (string)GetDefaultValue(TQAReportingQualityId));
			set => GetSetting<string>(TQAReportingQualityId, (string)GetDefaultValue(TQAReportingQualityId)).Value = value;
		}

		public TQAReportingSettings ResetToDefaults()
		{
			TQAReportingQuality = (string)GetDefaultValue(TQAReportingQualityId);
			return this;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case TQAReportingQualityId: return TQAReportingDefaultQuality;
			}

			return base.GetDefaultValue(settingId);
		}
	}
}

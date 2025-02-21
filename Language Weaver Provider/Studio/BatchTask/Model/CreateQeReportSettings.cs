using Sdl.Core.Settings;

namespace LanguageWeaverProvider.Studio.BatchTask.Model
{
	public class CreateQeReportSettings : SettingsGroup
	{
		public ISettingsGroup Settings { get; set; }

		public bool ExcludeLockedSegments
		{
			get { return GetSetting<bool>(nameof(ExcludeLockedSegments)); }
			set { GetSetting<bool>(nameof(ExcludeLockedSegments)).Value = value; }
		}

		protected override object GetDefaultValue(string settingId)
		{
			return base.GetDefaultValue(settingId);
		}
	}
}
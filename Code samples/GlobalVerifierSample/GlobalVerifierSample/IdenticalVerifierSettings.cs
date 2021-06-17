using Sdl.Core.Settings;

namespace GlobalVerifierSample
{
	public class IdenticalVerifierSettings : SettingsGroup
	{
		// Define the setting constant.
		private const string CheckContext_Setting = "CheckContext";

		// Return the value of the setting.
		public Setting<string> CheckContext
		{
			get { return GetSetting<string>(CheckContext_Setting); }
		}

		/// <summary>
		/// Return the default value of the setting property, i.e the context display code,
		/// which by default is H (i.e. headline).
		/// </summary>
		/// <param name="settingId"></param>
		/// <returns></returns>

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case "CheckContext":
					return (string)"H";

				default:
					return base.GetDefaultValue(settingId);
			}
		}
	}
}
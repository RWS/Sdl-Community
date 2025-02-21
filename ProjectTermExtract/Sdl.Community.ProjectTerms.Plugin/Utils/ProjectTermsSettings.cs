using Sdl.Core.Settings;

namespace Sdl.Community.ProjectTerms.Plugin.Utils
{
	public class ProjectTermsSettings :  SettingsGroup
	{		
		private const string ProjectTermOccurrencesId = "ProjectTermOccurrences";
		private const string ProjectTermLengthId = "ProjectTermLengthId";
		public Setting<int> TermOccurrences
		{
			get => GetSetting<int>(ProjectTermOccurrencesId);
			set => GetSetting<int>(ProjectTermOccurrencesId).Value = value;
		}
		public Setting<int> TermLength
		{
			get => GetSetting<int>(ProjectTermLengthId);
			set => GetSetting<int>(ProjectTermLengthId).Value = value;
		}
		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case ProjectTermOccurrencesId:
					return 0;
				case ProjectTermLengthId:
					return 0;
				default:
					return base.GetDefaultValue(settingId);
			}
		}
	}
}

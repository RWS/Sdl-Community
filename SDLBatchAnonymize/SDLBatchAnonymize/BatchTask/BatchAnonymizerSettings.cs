using Sdl.Community.SDLBatchAnonymize.Interface;
using Sdl.Core.Settings;

namespace Sdl.Community.SDLBatchAnonymize.BatchTask
{
	public class BatchAnonymizerSettings:SettingsGroup,IBatchAnonymizerSettings
	{
		public bool AnonymizeComplete
		{
			get => GetSetting<bool>(nameof(AnonymizeComplete));
			set => GetSetting<bool>(nameof(AnonymizeComplete)).Value = value;
		}
		public bool AnonymizeTmMatch {
			get => GetSetting<bool>(nameof(AnonymizeTmMatch));
			set => GetSetting<bool>(nameof(AnonymizeTmMatch)).Value = value;
		}
		public string FuzzyScore {
			get => GetSetting<string>(nameof(FuzzyScore));
			set => GetSetting<string>(nameof(FuzzyScore)).Value = value;
		}
		public string TmName
		{
			get => GetSetting<string>(nameof(TmName));
			set => GetSetting<string>(nameof(TmName)).Value = value;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case nameof(AnonymizeComplete):
					return true;
			}
			return base.GetDefaultValue(settingId);
		}
	}
}

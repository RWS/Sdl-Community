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

		public bool CreatedByChecked
		{
			get => GetSetting<bool>(nameof(CreatedByChecked));
			set => GetSetting<bool>(nameof(CreatedByChecked)).Value = value;
		}
		public string CreatedByName
		{
			get => GetSetting<string>(nameof(CreatedByName));
			set => GetSetting<string>(nameof(CreatedByName)).Value = value;
		}
		public bool ModifyByChecked
		{
			get => GetSetting<bool>(nameof(ModifyByChecked));
			set => GetSetting<bool>(nameof(ModifyByChecked)).Value = value;
		}
		public string ModifyByName
		{
			get => GetSetting<string>(nameof(ModifyByName));
			set => GetSetting<string>(nameof(ModifyByName)).Value = value;
		}

		public bool CommentChecked
		{
			get => GetSetting<bool>(nameof(CommentChecked));
			set => GetSetting<bool>(nameof(CommentChecked)).Value = value;
		}

		public string CommentAuthorName
		{
			get => GetSetting<string>(nameof(CommentAuthorName));
			set => GetSetting<string>(nameof(CommentAuthorName)).Value = value;
		}

		public bool TrackedChecked
		{
			get => GetSetting<bool>(nameof(TrackedChecked));
			set => GetSetting<bool>(nameof(TrackedChecked)).Value = value;
		}

		public string TrackedName
		{
			get => GetSetting<string>(nameof(TrackedName));
			set => GetSetting<string>(nameof(TrackedName)).Value = value;
		}
		public bool ChangeMtChecked
		{
			get => GetSetting<bool>(nameof(ChangeMtChecked));
			set => GetSetting<bool>(nameof(ChangeMtChecked)).Value = value;
		}
		public bool ChangeTmChecked
		{
			get => GetSetting<bool>(nameof(ChangeTmChecked));
			set => GetSetting<bool>(nameof(ChangeTmChecked)).Value = value;
		}
		public bool SetSpecificResChecked
		{
			get => GetSetting<bool>(nameof(SetSpecificResChecked));
			set => GetSetting<bool>(nameof(SetSpecificResChecked)).Value = value;
		}

		public decimal FuzzyScore {
			get => GetSetting<decimal>(nameof(FuzzyScore));
			set => GetSetting<decimal>(nameof(FuzzyScore)).Value = value;
		}
		public string TmName
		{
			get => GetSetting<string>(nameof(TmName));
			set => GetSetting<string>(nameof(TmName)).Value = value;
		}
	}
}

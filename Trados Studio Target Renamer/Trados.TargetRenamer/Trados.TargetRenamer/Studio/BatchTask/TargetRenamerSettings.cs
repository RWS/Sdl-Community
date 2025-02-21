using Sdl.Core.Settings;
using Trados.TargetRenamer.Interfaces;

namespace Trados.TargetRenamer.BatchTask
{
	public class TargetRenamerSettings : SettingsGroup, ITargetRenamerSettings
	{
		public bool AppendAsPrefix
		{
			get => GetSetting<bool>(nameof(AppendAsPrefix));
			set => GetSetting<bool>(nameof(AppendAsPrefix)).Value = value;
		}

		public bool AppendAsSuffix
		{
			get => GetSetting<bool>(nameof(AppendAsSuffix));
			set => GetSetting<bool>(nameof(AppendAsSuffix)).Value = value;
		}

		public bool AppendCustomString
		{
			get => GetSetting<bool>(nameof(AppendCustomString));
			set => GetSetting<bool>(nameof(AppendCustomString)).Value = value;
		}

		public bool AppendTargetLanguage
		{
			get => GetSetting<bool>(nameof(AppendTargetLanguage));
			set => GetSetting<bool>(nameof(AppendTargetLanguage)).Value = value;
		}

		public string CustomLocation
		{
			get => GetSetting<string>(nameof(CustomLocation));
			set => GetSetting<string>(nameof(CustomLocation)).Value = value;
		}

		public string CustomString
		{
			get => GetSetting<string>(nameof(CustomString));
			set => GetSetting<string>(nameof(CustomString)).Value = value;
		}

		public string Delimiter
		{
			get => GetSetting<string>(nameof(Delimiter));
			set => GetSetting<string>(nameof(Delimiter)).Value = value;
		}

		public string RegularExpressionReplaceWith
		{
			get => GetSetting<string>(nameof(RegularExpressionReplaceWith));
			set => GetSetting<string>(nameof(RegularExpressionReplaceWith)).Value = value;
		}

		public string RegularExpressionSearchFor
		{
			get => GetSetting<string>(nameof(RegularExpressionSearchFor));
			set => GetSetting<string>(nameof(RegularExpressionSearchFor)).Value = value;
		}

		public bool UseCustomLocation
		{
			get => GetSetting<bool>(nameof(UseCustomLocation));
			set => GetSetting<bool>(nameof(UseCustomLocation)).Value = value;
		}

		public bool UseRegularExpression
		{
			get => GetSetting<bool>(nameof(UseRegularExpression));
			set => GetSetting<bool>(nameof(UseRegularExpression)).Value = value;
		}

		public bool UseShortLocales
		{
			get => GetSetting<bool>(nameof(UseShortLocales));
			set => GetSetting<bool>(nameof(UseShortLocales)).Value = value;
		}
	}
}
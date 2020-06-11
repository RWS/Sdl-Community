using System;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Core.Settings;

namespace Sdl.Community.XLIFF.Manager.Model.ProjectSettings
{
	public class XliffManagerProjectFileActivity : SettingsGroup
	{
		private const string ProjectFileIdSetting = "ProjectFileId";
		private const string StatusSetting = "Status";
		private const string ActionSetting = "Action";
		private const string ActivityIdSetting = "ActivityId";
		private const string NameSetting = "Name";
		private const string PathSetting = "Path";
		private const string DateSetting = "Date";
		private const string DetailsSetting = "Details";

		public XliffManagerProjectFileActivity()
		{
			Status.Value = Enumerators.Status.None.ToString();
			Action.Value = Enumerators.Action.None.ToString();
		}

		public Setting<string> ProjectFileId
		{
			get => GetSetting<string>(ProjectFileIdSetting);
			set => GetSetting<string>(ProjectFileIdSetting).Value = value;
		}

		public Setting<string> Status
		{
			get => GetSetting<string>(StatusSetting);
			set => GetSetting<string>(StatusSetting).Value = value;
		}

		public Setting<string> Action
		{
			get => GetSetting<string>(ActionSetting);
			set => GetSetting<string>(ActionSetting).Value = value;
		}

		public Setting<string> ActivityId
		{
			get => GetSetting<string>(ActivityIdSetting);
			set => GetSetting<string>(ActivityIdSetting).Value = value;
		}

		public Setting<string> Name
		{
			get => GetSetting<string>(NameSetting);
			set => GetSetting<string>(NameSetting).Value = value;
		}

		public Setting<string> Path
		{
			get => GetSetting<string>(PathSetting);
			set => GetSetting<string>(PathSetting).Value = value;
		}

		public Setting<string> Date
		{
			get => GetSetting<string>(DateSetting);
			set => GetSetting<string>(DateSetting).Value = value;
		}

		public Setting<string> Details
		{
			get => GetSetting<string>(DetailsSetting);
			set => GetSetting<string>(DetailsSetting).Value = value;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case ProjectFileIdSetting:
					return string.Empty;
				case StatusSetting:
					return Enumerators.Status.None.ToString();
				case ActionSetting:
					return Enumerators.Action.None.ToString();
				case ActivityIdSetting:
					return string.Empty;
				case NameSetting:
					return string.Empty;
				case PathSetting:
					return string.Empty;
				case DateSetting:
					return FormatDateTime(DateTime.MinValue);
				case DetailsSetting:
					return string.Empty;
				default:
					return base.GetDefaultValue(settingId);
			}
		}

		private static string FormatDateTime(DateTime dateTime)
		{
			var value = dateTime.Year
			            + "-" + dateTime.Month.ToString().PadLeft(2, '0')
			            + "-" + dateTime.Day.ToString().PadLeft(2, '0')
			            + "T" + dateTime.Hour.ToString().PadLeft(2, '0')
			            + ":" + dateTime.Minute.ToString().PadLeft(2, '0')
			            + ":" + dateTime.Second.ToString().PadLeft(2, '0')
			            + "." + dateTime.Millisecond.ToString().PadLeft(2, '0');

			return value;
		}
	}
}

using System;
using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Core.Settings;

namespace Sdl.Community.XLIFF.Manager.Model.ProjectSettings
{
	public class XliffManagerProjectFile :  SettingsGroup
	{
		private const string ProjectIdSetting = "ProjectId";
		private const string ActivitiesSetting = "Activities";
		private const string ActionSetting = "Action";
		private const string StatusSetting = "Status";
		private const string FileTypeSetting = "FileType";
		private const string FileIdSetting = "FileId";
		private const string TargetLanguageSetting = "TargetLanguage";
		private const string NameSetting = "Name";
		private const string PathSetting = "Path";
		private const string LocationSetting = "Location";
		private const string DateSetting = "Date";
		private const string DetailsSetting = "Details";
		private const string XliffFilePathSetting = "XliffFilePath";
		private const string ShortMessageSetting = "ShortMessage";

		public XliffManagerProjectFile()
		{
			Status.Value = Enumerators.Status.None.ToString();
			Action.Value = Enumerators.Action.None.ToString();
			Activities.Value = new List<XliffManagerProjectFileActivity>();
		}
		
		public Setting<List<XliffManagerProjectFileActivity>> Activities
		{
			get => GetSetting<List<XliffManagerProjectFileActivity>>(ActivitiesSetting);
			set => GetSetting<List<XliffManagerProjectFileActivity>>(ActivitiesSetting).Value = value;
		}

		public Setting<string> ProjectId
		{
			get => GetSetting<string>(ProjectIdSetting);
			set => GetSetting<string>(ProjectIdSetting).Value = value;
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

		public Setting<string> FileId
		{
			get => GetSetting<string>(FileIdSetting);
			set => GetSetting<string>(FileIdSetting).Value = value;
		}

		public Setting<string> TargetLanguage
		{
			get => GetSetting<string>(TargetLanguageSetting);
			set => GetSetting<string>(TargetLanguageSetting).Value = value;
		}

		public Setting<string> FileType
		{
			get => GetSetting<string>(FileTypeSetting);
			set => GetSetting<string>(FileTypeSetting).Value = value;
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

		public Setting<string> Location
		{
			get => GetSetting<string>(LocationSetting);
			set => GetSetting<string>(LocationSetting).Value = value;
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

		public Setting<string> XliffFilePath
		{
			get => GetSetting<string>(XliffFilePathSetting);
			set => GetSetting<string>(XliffFilePathSetting).Value = value;
		}

		public Setting<string> ShortMessage
		{
			get => GetSetting<string>(ShortMessageSetting);
			set => GetSetting<string>(ShortMessageSetting).Value = value;
		}
		
		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case ProjectIdSetting:
					return string.Empty;
				case StatusSetting:
					return Enumerators.Status.None.ToString();
				case ActionSetting:
					return Enumerators.Action.None.ToString();
				case FileIdSetting:
					return string.Empty;
				case NameSetting:
					return string.Empty;
				case PathSetting:
					return string.Empty;
				case LocationSetting:
					return string.Empty;
				case DateSetting:
					return FormatDateTime(DateTime.MinValue);
				case DetailsSetting:
					return string.Empty;
				case XliffFilePathSetting:
					return string.Empty;
				case ShortMessageSetting:
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

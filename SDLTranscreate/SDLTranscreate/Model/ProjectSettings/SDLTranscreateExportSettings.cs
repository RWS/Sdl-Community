using System;
using Sdl.Core.Settings;

namespace Trados.Transcreate.Model.ProjectSettings
{
	public class SDLTranscreateExportSettings : SettingsGroup
	{
		private const string ExportOptionsSettingId = "ExportOptions";
		private const string TransactionFolderSettingId = "TransactionFolder";
		private const string DateTimeStampSettingId = "DateTimeStamp";
		private const string LocalProjectFolderSettingId = "LocalProjectFolder";
		
		public string LocalProjectFolder
		{
			get => GetSetting<string>(LocalProjectFolderSettingId);
			set => GetSetting<string>(LocalProjectFolderSettingId).Value = value;
		}

		public ExportOptions ExportOptions
		{
			get => GetSetting<ExportOptions>(ExportOptionsSettingId);
			set => GetSetting<ExportOptions>(ExportOptionsSettingId).Value = value;
		}
		
		public DateTime DateTimeStamp
		{
			get => GetSetting<DateTime>(DateTimeStampSettingId);
			set => GetSetting<DateTime>(DateTimeStampSettingId).Value = value;
		}

		public string TransactionFolder
		{
			get => GetSetting<string>(TransactionFolderSettingId);
			set => GetSetting<string>(TransactionFolderSettingId).Value = value;
		}		
	}
}

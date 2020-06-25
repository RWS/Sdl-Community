using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Core.Settings;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	public class ImportSettings : SettingsGroup
	{
		private const string ImportOptionsSettingId = "ImportOptions";
		private const string TransactionFolderSettingId = "TransactionFolder";
		private const string ExcludeFilterItemIdsSettingId = "ExcludeFilterItemIds";

		public ImportOptions ImportOptions
		{
			get => GetSetting<ImportOptions>(ImportOptionsSettingId);
			set => GetSetting<ImportOptions>(ImportOptionsSettingId).Value = value;
		}
		
		public string TransactionFolder
		{
			get => GetSetting<string>(TransactionFolderSettingId);
			set => GetSetting<string>(TransactionFolderSettingId).Value = value;
		}

		public List<string> ExcludeFilterItemIds
		{
			get => GetSetting<List<string>>(ExcludeFilterItemIdsSettingId);
			set => GetSetting<List<string>>(ExcludeFilterItemIdsSettingId).Value = value;
		}
	}
}

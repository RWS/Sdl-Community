using System;

namespace Sdl.Community.BackupService.Helpers
{
	public static class Constants
	{
		public const int MandatoryActionColumnIndex = 0;
		public const int MandatoryTypeColumnIndex = 1;
		public const int MandatoryPatternColumnIndex = 2;

		public const string MandatoryValue = "Mandatory value";
		public const string ActionAlreadyExist = "Action already exist. If you want to add a new action please try with a different name, type and pattern.";
		public const string InformativeMessage = "Informative Message";

		public const string ManuallyOption = "Backup process started manually.";

		public const string InformativeErrorMessage = "Files were not copied correctly. Please check the backup settings and try again!";
		public const string BackupToErrorMessage = "'Backup to' cannot be empty. Please select folder where to backup files!";
		public const string BackupFromErrorMessage = "'Backup from' cannot be empty. Please select folder from where to backup files!";
		public const string IntervalErrorMessage = "Settings cannot be saved. Please set up a backup time value!";

		public static readonly string DeployPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL Community\TMBackup", Environment.UserName);
		public static readonly string SdlCommunityPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL Community", Environment.UserName);

		public static readonly string RegistryParam = " / WindowsInitialize";
	}
}
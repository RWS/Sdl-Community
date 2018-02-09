using System;

namespace Sdl.Community.BackupService.Helpers
{
	public static class Constants
	{
		public const int MandatoryActionColumnIndex = 0;
		public const int MandatoryTypeColumnIndex = 1;
		public const int MandatoryPatternColumnIndex = 2;

		public const string MandatoryValue = "Mandatory value";
		public const string ActionAlreadyExist = "Action with that specific pattern already exist!";
		public const string TaskSchedulerAlreadyExist = "Task already exist! If you want to add a new task please select a different name and scheduler settings.";
		public const string InformativeMessage = "Informative Message";

		public const string ManuallyOption = "Backup process started manually.";

		public const string InformativeErrorMessage = "Files were not copied correctly. Please check the backup settings and try again!";
		public const string IntervalErrorMessage = "Settings cannot be saved. Please set up a backup time value!";
		public const string BackupIntervalErrorMessage = "Backup interval field accepts only numbers!";
		public const string ActionErrorMessage = "Actions cannot be saved! Please ensure that Action name, Type of file and Pattern are filled!";

		public const string TaskNameErrorMessage = "Actions cannot be added because Backup name is empty! Please fill first the Backup name and after that add actions!";

		public const string ActionNameErrorMessage = "Actions cannot be saved! Please ensure that Action name is filled!";
		public const string FileTypeErrorMessage = "Actions cannot be saved! Please ensure that Type of file is filled!";
		public const string PatternErrorMessage = "Actions cannot be saved! Please ensure that Patern is filled!";

		public const string UpdateActionMessage = "Please ensure that all fields are filled in order to update the action!";

		public static readonly string DeployPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL Community\TMBackup", Environment.UserName);
		public static readonly string SdlCommunityPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL Community", Environment.UserName);

		public static readonly string TaskDetailValue = "TMBackupTask ";

		public static readonly string RegistryParam = " / WindowsInitialize";
	}
}
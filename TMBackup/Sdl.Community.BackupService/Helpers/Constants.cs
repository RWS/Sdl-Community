using System;
using System.IO;

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
		public const string DeleteInformativeMessage = "Are you sure you want to delete selected task(s)?";

		public const string ManuallyOption = "Backup process started manually.";

		public const string InformativeErrorMessage = "Files were not copied correctly. Please check the backup settings and try again!";
		public const string IntervalErrorMessage = "Settings cannot be saved. Please set up a backup time value!";
		public const string BackupIntervalErrorMessage = "Backup interval field accepts only numbers!";

		public const string TaskNameErrorMessage = "Actions cannot be added because the backup name is empty! Please fill first the backup name and after that add actions!";

		public const string ActionNameErrorMessage = "Action cannot be saved! Please ensure that action name is filled!";
		public const string FileTypeErrorMessage = "Action cannot be saved! Please ensure that type of file is filled!";
		public const string PatternErrorMessage = "Action cannot be saved! Please ensure that patern is filled!";

		public const string UpdateActionMessage = "Please ensure that all fields are filled in order to update the action!";
		
		public static readonly string DeployPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SDL Community\TMBackup");
		public static readonly string SdlCommunityPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SDL Community");

		public static readonly string TaskDetailValue = "TMBackupTask ";

		public static readonly string RegistryParam = " / WindowsInitialize";

		public static readonly string TimeFormat = "HH:mm:ss";
	}
}
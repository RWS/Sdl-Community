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

		public const string RunOption = "Run as soon as it becomes possible";
		public const string WaitOption = "Wait until next scheduled time and run only then";

		public const string ManuallyOption = "Process will be started manually.";


		public const string InformativeErrorMessage = "Files were not copied correctly. Please check the backup settings and try again!";

		public static readonly string DeployPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL Community\TMBackup", Environment.UserName);

		public static readonly string RegistryParam = " /WindowsInitialize";
	}
}
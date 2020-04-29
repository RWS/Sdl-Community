namespace Sdl.Community.BackupService.Models
{
	public class ChangeSettingsModel
	{
		public string BackupName { get; set; }
		public string TrimmedBackupName { get; set; }
		public bool IsPeriodicOptionChecked { get; set; }
		public bool IsManuallyOptionChecked { get; set; }
	}
}
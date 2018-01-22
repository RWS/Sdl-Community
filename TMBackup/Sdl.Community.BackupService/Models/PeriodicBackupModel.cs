using System;

namespace Sdl.Community.BackupService.Models
{
	public class PeriodicBackupModel
	{
		public string BackupName { get; set; }

		public int BackupInterval { get; set; }

		public string TimeType { get; set; }

		public DateTime FirstBackup { get; set; }

		public string BackupAt { get; set; }
	}
}
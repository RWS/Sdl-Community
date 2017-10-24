using System;

namespace Sdl.Community.TMBackup.Models
{
	public class PeriodicBackupModel
	{
		public int BackupInterval { get; set; }

		public string TimeType { get; set; }

		public DateTime FirstBackup { get; set; }

		public DateTime BackupAt { get; set; }

		public bool IsRunOption { get; set; }

		public bool IsWaitOption { get; set; }
	}
}
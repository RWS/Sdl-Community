using System.Collections.Generic;

namespace Sdl.Community.BackupService.Models
{
	public class JsonRequestModel
	{
		public List<BackupDetailsModel> BackupDetailsModelList { get; set; }

		public BackupModel BackupModel { get; set; }

		public ChangeSettingsModel ChangeSettingsModel { get; set; }

		public PeriodicBackupModel PeriodicBackupModel { get; set; }

		public RealTimeBackupModel RealTimeBackupModel { get; set; }
	}
}
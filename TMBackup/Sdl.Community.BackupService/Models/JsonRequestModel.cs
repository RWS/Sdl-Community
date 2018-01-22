using System.Collections.Generic;

namespace Sdl.Community.BackupService.Models
{
	public class JsonRequestModel
	{
		public List<BackupDetailsModel> BackupDetailsModelList { get; set; }

		public List<BackupModel> BackupModelList { get; set; }

		public List<ChangeSettingsModel> ChangeSettingsModelList { get; set; }

		public List<PeriodicBackupModel> PeriodicBackupModelList { get; set; }
	}
}
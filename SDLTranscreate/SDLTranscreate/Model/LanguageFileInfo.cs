using System.Collections.Generic;

namespace Trados.Transcreate.Model
{
	public class LanguageFileInfo
	{
		public LanguageFileInfo()
		{
			Guid = string.Empty;
			SettingsBundleGuid = string.Empty;
			LanguageCode = string.Empty;
			FileVersionInfos = new List<FileVersionInfo>();
			FileAssignmentInfos = new List<FileAssignmentInfo>();
		}

		public string Guid { get; set; }

		public string SettingsBundleGuid { get; set; }

		public string LanguageCode { get; set; }

		public List<FileVersionInfo> FileVersionInfos { get; set; }

		public List<FileAssignmentInfo> FileAssignmentInfos { get; set; }
	}
}

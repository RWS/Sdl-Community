using System;

namespace Trados.Transcreate.Model
{
	public class FileVersionInfo
	{
		public FileVersionInfo()
		{
			Guid = string.Empty;
			VersionNumber = 0;
			Size = 0;
			FileName = string.Empty;
			PhysicalPath = string.Empty;
			CreatedAt = DateTime.MinValue;
			CreatedBy = string.Empty;
			FileTimeStamp = DateTime.MinValue;
			IsAutoUpload = false;
		}

		public string Guid { get; set; }

		public int VersionNumber { get; set; }

		public long Size { get; set; }

		public string FileName { get; set; }

		public string PhysicalPath { get; set; }

		public DateTime CreatedAt { get; set; }

		public string CreatedBy { get; set; }

		public DateTime FileTimeStamp { get; set; }

		public bool IsAutoUpload { get; set; }
	}
}

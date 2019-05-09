using System;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class GsFileVersion
	{
		public string LanguageFileId { get; set; }
		public string FileId { get; set; }
		public string FileName { get; set; }
		public int Version { get; set; }
		public DateTime? LastModified { get; set; }
		public string CreatedBy { get; set; }
		public string CreatedAt { get; set; }
		public string CheckInComment { get; set; }
		public string ProjectId { get; set; }
	}
}

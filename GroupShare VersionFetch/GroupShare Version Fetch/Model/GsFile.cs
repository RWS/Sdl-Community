using System;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class GsFile
	{
		public Guid UniqueId { get; set; }
		public string FileName { get; set; }
		public int FileSize { get; set; }
		public string Status { get; set; }
		public string LanguageCode { get; set; }
		public string FileType { get; set; }
	}
}

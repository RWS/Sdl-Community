using System;
using System.Drawing;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class GsFile : BaseModel
	{
		public Guid UniqueId { get; set; }
		public string FileName { get; set; }
		public int FileSize { get; set; }
		public string Status { get; set; }
		public string LanguageCode { get; set; }
		public Image LanguageFlagImage { get; set; }
		public string FileType { get; set; }
		public string ProjectId { get; set; }
	}
}

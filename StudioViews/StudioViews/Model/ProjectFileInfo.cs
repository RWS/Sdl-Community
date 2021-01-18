using System.Collections.Generic;

namespace Sdl.Community.StudioViews.Model
{
	public class ProjectFileInfo
	{
		public ProjectFileInfo()
		{
			ParagraphInfos = new List<ParagraphInfo>();
			ContextDefinitions = new List<ParagraphUnitContext>();
		}

		public string FileId { get; set; }

		public string OriginalFilePath { get; set; }

		public string FileTypeId { get; set; }

		public string Original { get; set; }

		public string SourceLanguage { get; set; }

		public string TargetLanguage { get; set; }

		public bool IsMergedFile { get; set; }

		public List<ParagraphInfo> ParagraphInfos { get; set; }

		public List<ParagraphUnitContext> ContextDefinitions { get; set; }
	}
}

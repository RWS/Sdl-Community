using Sdl.Community.StudioViews.Interfaces;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.StudioViews.Model
{
	public class SegmentInfoBase : ISegmentInfo
	{
		public string FileId { get; set; }

		public string ParagraphUnitId { get; set; }

		public string SegmentId { get; set; }

		public IParagraphUnit ParagraphUnit { get; set; }

		public WordCounts SourceWordCounts { get; set; }
	}
}

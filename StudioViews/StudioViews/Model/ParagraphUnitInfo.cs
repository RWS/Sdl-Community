using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.StudioViews.Model
{
	public class ParagraphUnitInfo
	{
		public IParagraphUnit ParagraphUnit { get; set; }

		public string FileId { get; set; }
		
		public List<SegmentPairInfo> SegmentPairs { get; set; }
	}
}

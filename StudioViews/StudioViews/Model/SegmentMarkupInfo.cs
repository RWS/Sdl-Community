using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.StudioViews.Model
{
	public class SegmentMarkupInfo : SegmentInfoBase
	{
		public List<IAbstractMarkupData> SourceMarkupData { get; set; }

		public List<IAbstractMarkupData> TargetMarkupData { get; set; }
		
	}
}

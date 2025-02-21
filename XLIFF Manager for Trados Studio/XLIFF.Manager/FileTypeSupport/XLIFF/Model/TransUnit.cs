using System.Collections.Generic;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model
{
	public class TransUnit
	{
		public TransUnit()
		{
			SegmentPairs = new List<SegmentPair>();
		}

		public string Id { get; set; }

		public List<SegmentPair> SegmentPairs { get; set; }
	}
}

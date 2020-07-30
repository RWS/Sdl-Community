using System;
using System.Collections.Generic;

namespace Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class TransUnit : ICloneable
	{
		public TransUnit()
		{
			SegmentPairs = new List<SegmentPair>();
		}

		public string Id { get; set; }

		public List<SegmentPair> SegmentPairs { get; set; }

		public object Clone()
		{
			var transUnit = new TransUnit
			{
				Id = Id
			};
			foreach (var segmentPair in SegmentPairs)
			{
				transUnit.SegmentPairs.Add(segmentPair.Clone() as SegmentPair);
			}

			return transUnit;
		}
	}
}

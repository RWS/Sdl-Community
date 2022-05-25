using System;
using System.Collections.Generic;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class TransUnit : ICloneable
	{
		public TransUnit()
		{
			SegmentPairs = new List<SegmentPair>();
			Contexts = new List<Context>();
		}

		public string Id { get; set; }

		public List<SegmentPair> SegmentPairs { get; set; }

		public List<Context> Contexts { get; set; }

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

			foreach (var context in Contexts)
			{
				transUnit.Contexts.Add(context.Clone() as Context);
			}

			return transUnit;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.TermRecognition
{
	public class FeatureToRangeMapping
	{
		private int _Feature;
		private Core.SegmentRange _Range;

		public FeatureToRangeMapping(int feature, Core.SegmentRange range)
		{
			_Feature = feature;
			_Range = range;
		}

		public int Feature
		{
			get { return _Feature; }
			set { _Feature = value; }
		}

		public Core.SegmentRange Range
		{
			get { return _Range; }
			set { _Range = value; }
		}

	}
}

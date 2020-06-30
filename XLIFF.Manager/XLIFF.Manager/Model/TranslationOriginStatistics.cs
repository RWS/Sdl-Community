using System;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class TranslationOriginStatistics : ICloneable
	{
		public TranslationOriginStatistics()
		{			
			WordCounts = new WordCounts();
		}

		public WordCounts WordCounts { get; set; }

		public object Clone()
		{
			var stats = new TranslationOriginStatistics
			{
				WordCounts = WordCounts.Clone() as WordCounts
			};

			return stats;
		}
	}
}

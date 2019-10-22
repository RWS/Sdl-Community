using System;
using System.Linq;
using Sdl.Community.AdvancedDisplayFilter.Models;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;
using static System.Int32;


namespace Sdl.Community.AdvancedDisplayFilter.Helpers
{
	public static class FuzzyHelper
	{
		public static bool IsInFuzzyRange(DisplayFilterRowInfo row, string fuzzyMin, string fuzzyMax)
		{
			try
			{
				var segmentMatchPercent = Parse(row.SegmentPair.Properties.TranslationOrigin.MatchPercent.ToString());

				var fuzzyRange = new SegmentRange
				{
					Min = Parse(fuzzyMin),
					Max = Parse(fuzzyMax)
				};
				if (Enumerable.Range(fuzzyRange.Min, fuzzyRange.Max - fuzzyRange.Min + 1).Contains(segmentMatchPercent))
				{
					return true;
				}
			}
			catch
			{
				// catch all; ignore
			}

			return false;
		}

		public static bool IsEditedFuzzyMatch(ITranslationOrigin translationOrigin)
		{
			if (translationOrigin?.OriginBeforeAdaptation != null)
			{
				var originType = translationOrigin.OriginType;
				if (string.Compare(originType, "interactive", StringComparison.InvariantCultureIgnoreCase) == 0
					&& IsFuzzyMatch(translationOrigin.OriginBeforeAdaptation))
				{
					return true;
				}
			}

			return false;
		}

		public static bool ContainsFuzzyMatch(ITranslationOrigin translationOrigin)
		{			
			return IsFuzzyMatch(translationOrigin);
		}

		private static bool IsFuzzyMatch(ITranslationOrigin translationOrigin)
		{
			var originType = translationOrigin?.OriginType;
			if (string.Compare(originType, "mt", StringComparison.InvariantCultureIgnoreCase) == 0 
			    || string.Compare(originType, "nmt", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				return false;
			}

			var match = translationOrigin?.MatchPercent ?? 0;
			return match != 0 && match < 100;
		}
	}
}

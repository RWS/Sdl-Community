using System;
using System.Linq;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;
using static System.Int32;


namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public static class FuzzyHelper
	{
		public static bool IsInFuzzyRange(DisplayFilterRowInfo row,string fuzzyMin,string fuzzyMax)
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
			catch (Exception e)
			{
				// ignored
			}

			return false;
		}

		public static bool IsEditedFuzzy(ISegment segment)
		{
			if (!ContainsFuzzy(segment))
			{
				return false;
			}
			//for 100% edited
			if ((bool)segment.Properties?.TranslationOrigin?.OriginType.Equals("auto-propagated"))
			{
				return true;
			}

			if (segment.Properties?.TranslationOrigin?.OriginBeforeAdaptation?.OriginBeforeAdaptation != null)
			{
				//for 100% fuzzy which is not edited but is picked as edited
				if (segment.Properties.TranslationOrigin.TextContextMatchLevel.ToString().Equals("Source"))
				{
					return false;
				}

				if (segment.Properties.TranslationOrigin.OriginBeforeAdaptation.OriginBeforeAdaptation.OriginType != "source")
				{
					return true;
				}
			}
			return false;
		}

		private static bool ContainsFuzzy(ISegment segment)
		{
			if (segment.Properties?.TranslationOrigin?.OriginBeforeAdaptation == null)
			{
				if ((bool)segment.Properties?.TranslationOrigin.MatchPercent.Equals(0))
				{
					return false;
				}
			}
			else
			{
				if (segment.Properties?.TranslationOrigin?.OriginBeforeAdaptation?.OriginBeforeAdaptation == null)
				{
					if ((bool)segment.Properties?.TranslationOrigin?.OriginBeforeAdaptation?.MatchPercent.Equals(0))
					{
						return false;
					}
				}
				else
				{
					if (segment.Properties?.TranslationOrigin?.OriginBeforeAdaptation?.OriginBeforeAdaptation?.OriginType != null)
					{
						if ((bool)segment.Properties?.TranslationOrigin?.OriginBeforeAdaptation?.OriginBeforeAdaptation?.OriginType
							.Equals("source"))
						{
							return false;
						}
					}
					if ((bool)segment.Properties?.TranslationOrigin?.OriginBeforeAdaptation?.OriginType.Equals("source"))
					{
						return false;
					}
				}

			}
			return true;
		}

	}
}

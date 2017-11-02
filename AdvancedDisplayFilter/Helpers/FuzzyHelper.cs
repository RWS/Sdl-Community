using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.Plugins.AdvancedDisplayFilter.DisplayFilters;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Models;
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

	}
}

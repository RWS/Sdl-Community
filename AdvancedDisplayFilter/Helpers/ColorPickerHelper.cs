using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public static class ColorPickerHelper
	{
		public static bool ContainsColor(DisplayFilterRowInfo rowInfo, List<string> colorsCode)
		{
			var visitor = new TagDataVisitor();

			var colorCodes = visitor.GetTagsColorCode(rowInfo.SegmentPair.Source);
			foreach (var selectedColor in colorsCode)
			{
				//code has #ffffff form in Studio: ffffff
				if (colorCodes.Contains(selectedColor.Substring(1, selectedColor.Length - 1)))
				{
					return true;
				}
			}

			return false;
		}

		public static List<string> GetColorsList(ISegment segment)
		{
			var visitor = new TagDataVisitor();
			var colorCodes = visitor.GetTagsColorCode(segment);

			return colorCodes;
		}

		
	}
}

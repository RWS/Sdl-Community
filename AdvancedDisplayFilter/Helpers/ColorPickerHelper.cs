using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.Toolkit.FileType;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public static class ColorPickerHelper
	{
		public static bool ContainsColor(DisplayFilterRowInfo rowInfo,List<string> colorsCode)
		{
			//get text from segment including tags 
			var sourceText = rowInfo.SegmentPair.Source.GetString(true);

			foreach (var code in colorsCode)
			{
				if (sourceText.Contains(string.Format("color=\"{0}\"", code)))
				{
					return true;
				}
			}

			var targetText = rowInfo.SegmentPair.Target.GetString(true);
			foreach (var code in colorsCode)
			{
				if (targetText.Contains(string.Format("color=\"{0}\"", code)))
				{
					return true;
				}
			}
			return false;
		}

		public static string GetHexCode(byte red, byte green, byte blue)
		{
			var hexCode = string.Format("{0:X2}{1:X2}{2:X2}", red, green,blue);

			return hexCode;
		}

		public static bool ContainsColorForIdmlFileType(DisplayFilterRowInfo rowInfo, List<string> customSettingsColors)
		{
			var visitor = new TagDataVisitor();
			var colorCodes = visitor.GetTagsColorCode(rowInfo.SegmentPair.Source);
			//visitor.VisitSegment(rowInfo.SegmentPair.Source);
			//var test = rowInfo.SegmentPair.Source.AllSubItems.Cast<MarkupDataContainer>();
			//foreach (var a in test)
			//{
				
			//}
			//foreach (var location in rowInfo.SegmentPair.Source.AllSubItems)
			//{
			//	var test1 = (MarkupData)location.
			//	//foreach (IAbstractMarkupDataContainer level in location.)
			//	//{
			//	//	if (level.ItemAtLocation != null)
			//	//	{
			//	//		var test = level.ItemAtLocation.ParentParagraph.Locations;
			//	//		foreach (var a in test)
			//	//		{
			//	//			var b=a.Levels[0].Parent;
			//	//		}
			//	//		// var test = lever.ItemAtLocation.
			//	//	}
			//	//}
			//}
			return false;
		}
	}
}

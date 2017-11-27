using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.Toolkit.FileType;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public static class ColorPickerHelper
	{
		//public static bool ContainsColor(DisplayFilterRowInfo rowInfo,List<string> colorsCode)
		//{
		//	//get text from segment including tags 
		//	var sourceText = rowInfo.SegmentPair.Source.GetString(true);

		//	foreach (var code in colorsCode)
		//	{
		//		var color = code.Substring(1, code.Length - 1);
		//		if (sourceText.Contains(string.Format("color={0}", color)))
		//		{
		//			return true;
		//		}
		//	}

		//	var targetText = rowInfo.SegmentPair.Target.GetString(true);
		//	foreach (var code in colorsCode)
		//	{
		//		if (targetText.Contains(string.Format("color={0}", code.Substring(1, code.Length-1))))
		//		{
		//			return true;
		//		}
		//	}
		//	return false;
		//}


		public static bool ContainsColor(DisplayFilterRowInfo rowInfo, List<string> colorsCode)
		{
			//get text from segment including tags 
			//var sourceText = rowInfo.SegmentPair.Source.GetString(true);

			//foreach (var code in colorsCode)
			//{
			//	if (sourceText.Contains(string.Format("color=\"{0}\"", code.Substring(1, code.Length - 1))))
			//	{
			//		return true;
			//	}
			//}

			//var targetText = rowInfo.SegmentPair.Target.GetString(true);
			//foreach (var code in colorsCode)
			//{
			//	if (targetText.Contains(string.Format("color=\"{0}\"", code.Substring(1, code.Length - 1))))
			//	{
			//		return true;
			//	}
			//}
			//return false;

			var visitor = new TagDataVisitor();
			var colorCodes = visitor.GetTagsColorCode(rowInfo.SegmentPair.Source);
			foreach (var selectedColor in colorsCode)
			{
				if (colorCodes.Contains(selectedColor.Substring(1, selectedColor.Length - 1)))
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

			foreach (var selectedColor in customSettingsColors)
			{
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

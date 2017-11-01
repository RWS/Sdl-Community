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
			var text = rowInfo.SegmentPair.Source.GetString(true);

			foreach (var code in colorsCode)
			{
				if (text.Contains(string.Format("color=\"{0}\"", code)))
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
	}
}

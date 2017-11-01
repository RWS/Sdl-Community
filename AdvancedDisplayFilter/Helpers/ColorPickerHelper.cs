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
		public static bool ContainsColor(DisplayFilterRowInfo rowInfo)
		{
			//get text from segment including tags 
			var text = rowInfo.SegmentPair.Source.GetString(true);

			//if (text.Contains("color=\"4472C4\""))
			//{
			//	return true;
			//}

			return false;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public static class ColorPickerHelper
	{
		private static List<string> _selectedColorsCode = new List<string>();
		public static bool ContainsColor(DisplayFilterRowInfo rowInfo, List<string> colorsCode)
		{
			_selectedColorsCode = colorsCode;
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
			var colorTextWithoutTag=DefaultFormatingContainsColor(rowInfo);

			return colorTextWithoutTag;
		}

		private static bool DefaultFormatingContainsColor(DisplayFilterRowInfo rowInfo)
		{
			if (rowInfo.ContextInfo != null && rowInfo.ContextInfo[0].DefaultFormatting!=null)
			{
				if (rowInfo.ContextInfo[0].DefaultFormatting["TextColor"] != null)
				{
					var color = rowInfo.ContextInfo[0].DefaultFormatting["TextColor"].StringValue;
					if (rowInfo.ContextInfo[0].HasMetaData)
					{
						foreach (var metaData in rowInfo.ContextInfo[0].MetaData)
						{
							if(metaData.Key.Equals("node"))
							{
								var style = metaData.Value;
								//check if there is no style which overrides text color property
								if (style.Contains("No character style"))
								{
									var colors = color.Split(',');

									var red = string.Empty;
									var green = string.Empty;
									var blue = string.Empty;

									//for  files which color code is like this "0,12,12,12"
									if (colors.Count().Equals(4))
									{
										red = colors[1];
										green = colors[2];
										blue = colors[3];

										return ParseColorCode(red, green, blue);
									}
									//"12,12,12"
									if (colors.Count().Equals(3))
									{
										red = colors[0];
										green = colors[1];
										blue = colors[2];

										return ParseColorCode(red, green, blue);
									}
								}
							}
						}
					}
				}
		
			}
			return false;
		}

		public static bool ParseColorCode(string red, string green, string blue)
		{
			var redSuccess = byte.TryParse(red, out byte redByteCode);
			var greenSuccess = byte.TryParse(green, out byte greenByteCode);
			var blueSuccess = byte.TryParse(blue, out byte blueByteCode);

			if (redSuccess && greenSuccess && blueSuccess)
			{
				var hexCode = GetHexCode(redByteCode,greenByteCode,blueByteCode);

				if (_selectedColorsCode.Contains("#"+hexCode))
				{
					return true;
				}
			}
			return false;
		}

		public static string GetHexCode(byte red, byte green, byte blue)
		{
			var hexCode = $"{red:X2}{green:X2}{blue:X2}";

			return hexCode;
		}

		public static List<string> GetColorsList(ISegment segment)
		{
			var visitor = new TagDataVisitor();
			var colorCodes = visitor.GetTagsColorCode(segment);

			return colorCodes;
		}

		
	}
}

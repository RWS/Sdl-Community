using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
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
			var colorTextWithoutTag = DefaultFormatingColorCode(rowInfo.ContextInfo);
			var containsColor = ContainsColor(colorTextWithoutTag);

			return containsColor;
		}

		public static string DefaultFormatingColorCode(IList<IContextInfo> contextInfo)
		{
			if (contextInfo != null && contextInfo.Count > 0)
			{
				var defaultFormatting = contextInfo[0].DefaultFormatting;
				if (defaultFormatting != null )
				{
					if (contextInfo[0].DefaultFormatting["TextColor"] != null)
					{
						var color = defaultFormatting["TextColor"].StringValue;
						if (contextInfo[0].HasMetaData)
						{
							foreach (var metaData in contextInfo[0].MetaData)
							{
								if (metaData.Key.Equals("node"))
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
				var colorFromMetadata = GetColorFromMetadata(contextInfo[0]);
				return colorFromMetadata;
			}
			return string.Empty;
		}
		private static string GetColorFromMetadata(IContextInfo contextInfo)
		{
			if (contextInfo.HasMetaData)
			{
				if (contextInfo.MetaDataContainsKey("ParagraphFormatting"))
				{
					var paragraphValue = contextInfo.GetMetaData("ParagraphFormatting");
					if (paragraphValue.Contains("color"))
					{
						var regex = new Regex("(w:val=\"[0-9a-fA-F]*\")");
						var matches = regex.Matches(paragraphValue);
						foreach (Match match in matches)
						{
							var value = match.Value;
							//color string is like this "code" - we need to remove the "" characters
							var color = value.Substring(value.IndexOf('"') + 1).TrimEnd('"');
							return color;
						}
					}

				}
			}
			return string.Empty;
		}

		public static string ParseColorCode(string red, string green, string blue)
		{
			var redSuccess = byte.TryParse(red, out byte redByteCode);
			var greenSuccess = byte.TryParse(green, out byte greenByteCode);
			var blueSuccess = byte.TryParse(blue, out byte blueByteCode);

			if (redSuccess && greenSuccess && blueSuccess)
			{
				return  GetHexCode(redByteCode,greenByteCode,blueByteCode);
			}
			return string.Empty;
		}

		public static bool ContainsColor(string colorCode)
		{
			return _selectedColorsCode.Contains("#" + colorCode);
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

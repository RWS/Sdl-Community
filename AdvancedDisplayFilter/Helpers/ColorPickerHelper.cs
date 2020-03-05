using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Sdl.Community.AdvancedDisplayFilter.DisplayFilters;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.AdvancedDisplayFilter.Helpers
{
	public static class ColorPickerHelper
	{
		public static bool ContainsColor(DisplayFilterRowInfo rowInfo, List<string> colorsCodes, DisplayFilterSettings.ContentLocation colorsFoundIn)
		{
			try
			{				
				var colors = GetColors(rowInfo.SegmentPair, colorsFoundIn);
				
				foreach (var selectedColor in colors)
				{
					var colorCodeA = selectedColor.TrimStart('#');
					foreach (var color in colorsCodes)
					{
						var colorCodeB = color.TrimStart('#');
						if (string.Compare(colorCodeA, colorCodeB, StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							return true;
						}
					}
				}

				var colorTextWithoutTag = DefaultFormatingColorCode(rowInfo.ContextInfo);
				var containsColor = colorsCodes.Contains("#" + colorTextWithoutTag);

				return containsColor;
			}
			catch
			{
				// ignore; catch all
			}

			return false;
		}

		public static List<string> GetColors(ISegmentPair segmentPair, DisplayFilterSettings.ContentLocation foundIn)
		{
			var paragraphUnit = ColorPickerHelper.GetParagraphUnit(segmentPair);

			var colors = new List<string>();
			if (foundIn == DisplayFilterSettings.ContentLocation.SourceAndTarget)
			{
				var sourceColors = paragraphUnit != null
					? GetColorsList(paragraphUnit.Source, segmentPair.Source)
					: GetColorsList(segmentPair.Source);

				var targetColors = paragraphUnit != null
					? GetColorsList(paragraphUnit.Target, segmentPair.Target)
					: GetColorsList(segmentPair.Target);

				foreach (var color in sourceColors)
				{
					if (targetColors.Contains(color))
					{
						colors.Add(color);
					}
				}
			}
			else if (foundIn == DisplayFilterSettings.ContentLocation.SourceOrTarget)
			{
				var sourceColors = paragraphUnit != null
					? GetColorsList(paragraphUnit.Source, segmentPair.Source)
					: GetColorsList(segmentPair.Source);

				var targetColors = paragraphUnit != null
					? GetColorsList(paragraphUnit.Target, segmentPair.Target)
					: GetColorsList(segmentPair.Target);

				colors = sourceColors;

				foreach (var color in targetColors)
				{
					if (!colors.Contains(color))
					{
						colors.Add(color);
					}
				}
			}
			else if (foundIn == DisplayFilterSettings.ContentLocation.Source)
			{
				colors = paragraphUnit != null
					? GetColorsList(paragraphUnit.Source, segmentPair.Source)
					: GetColorsList(segmentPair.Source);
			}
			else if (foundIn == DisplayFilterSettings.ContentLocation.Target)
			{
				colors = paragraphUnit != null
					? GetColorsList(paragraphUnit.Target, segmentPair.Target)
					: GetColorsList(segmentPair.Target);
			}

			return colors;
		}


		public static string DefaultFormatingColorCode(IList<IContextInfo> contextInfo)
		{
			if (contextInfo != null && contextInfo.Count > 0)
			{
				var defaultFormatting = contextInfo[0].DefaultFormatting;
				if (defaultFormatting != null)
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
										string red;
										string green;
										string blue;

										//for  files which color code is like this "0,12,12,12"
										if (colors.Length.Equals(4))
										{
											red = colors[1];
											green = colors[2];
											blue = colors[3];

											return ParseColorCode(red, green, blue);
										}

										//"12,12,12"
										if (colors.Length.Equals(3))
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
				var colorCode = GetColorFromMetadata(contextInfo[0]);
				return colorCode;
			}

			return string.Empty;
		}

		private static string GetColorFromMetadata(IContextInfo contextInfo)
		{
			if (contextInfo.HasMetaData && contextInfo.MetaDataContainsKey("ParagraphFormatting"))
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

			return string.Empty;
		}

		public static string ParseColorCode(string red, string green, string blue)
		{
			var redSuccess = byte.TryParse(red, out byte redByteCode);
			var greenSuccess = byte.TryParse(green, out byte greenByteCode);
			var blueSuccess = byte.TryParse(blue, out byte blueByteCode);

			if (redSuccess && greenSuccess && blueSuccess)
			{
				return GetHexCode(redByteCode, greenByteCode, blueByteCode);
			}

			return string.Empty;
		}

		public static string GetHexCode(byte red, byte green, byte blue)
		{
			var hexCode = $"{red:X2}{green:X2}{blue:X2}";

			return hexCode;
		}

		public static IParagraphUnit GetParagraphUnit(ISegmentPair segmentPair)
		{
			var type = segmentPair.GetType();
			var memberInfo = type.GetMember("_segmentPair", BindingFlags.NonPublic | BindingFlags.Instance);
			if (memberInfo.Length > 0)
			{
				var fieldInfo = memberInfo[0] as FieldInfo;
				if (fieldInfo != null)
				{
					var iSegmentPair = fieldInfo.GetValue(segmentPair) as ISegmentPair;
					return iSegmentPair?.Source.ParentParagraphUnit;
				}
			}

			return null;
		}

		public static List<string> GetColorsList(IParagraph paragraph, ISegment segment)
		{
			var visitor = new TagDataVisitor();
			var colorCodes = visitor.GetTagsColorCode(paragraph, segment);

			return colorCodes;
		}

		public static List<string> GetColorsList(ISegment segment)
		{
			var visitor = new TagDataVisitor();
			var colorCodes = visitor.GetTagsColorCode(segment);

			return colorCodes;
		}
	}
}

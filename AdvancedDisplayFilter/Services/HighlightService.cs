using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Sdl.Community.AdvancedDisplayFilter.Helpers;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.AdvancedDisplayFilter.Services
{
	public class HighlightService
	{
		private const string CadfBackgroundColorKey = "CADFBackgroundColorKey";

		public enum HighlightScrope
		{
			Filtered,
			Active
		}

		public void ApplyHighlighting(Document document, HighlightScrope highlightScrope, Color color)
		{
			var segments = highlightScrope == HighlightScrope.Filtered
				? document?.FilteredSegmentPairs?.ToList()
				: new List<ISegmentPair> { document.GetActiveSegmentPair() };

			if (segments == null)
			{
				return;
			}

		

			var allTagIds = GetAllTagIds(document);
			var seed = GetLargestSeedValue(allTagIds);

			var colorName = GetColorName(color);
			var colorRgb = GetColorRgb(color);

			var itemFactory = document.ItemFactory;
			var propertyFactory = itemFactory.PropertiesFactory;
			var formattingFactory = propertyFactory.FormattingItemFactory;
			var formattingItem = formattingFactory.CreateFormattingItem("BackgroundColor", colorRgb);

			foreach (var segmentPair in segments)
			{
				var item = segmentPair.Target.AllSubItems.FirstOrDefault();
				if (item is ITagPair tagPair)
				{
					if (tagPair.StartTagProperties?.Formatting == null ||
						tagPair.StartTagProperties.Formatting.ContainsKey(CadfBackgroundColorKey))
					{
						continue;
					}
				}

				var tagId = GetNextTagId(allTagIds, seed);
				seed = tagId + 1;

				var startTagProperties = CreateStartTagProperties(propertyFactory, formattingFactory, formattingItem, tagId, colorName);
				var endTagProperties = CreateEndTagProperties(propertyFactory);

				var tagPairNew = document.ItemFactory.CreateTagPair(startTagProperties, endTagProperties);
				segmentPair.Target.MoveAllItemsTo(tagPairNew);
				segmentPair.Target.Add(tagPairNew);

				document.UpdateSegmentPair(segmentPair);
			}
		}	

		public void ClearHighlighting(Document document, HighlightScrope highlightScrope)
		{
			var segments = highlightScrope == HighlightScrope.Filtered
				? document?.FilteredSegmentPairs?.ToList()
				: new List<ISegmentPair> { document.GetActiveSegmentPair() };

			if (segments == null)
			{
				return;
			}

			foreach (var segmentPair in segments)
			{
				var item = segmentPair.Target.AllSubItems.FirstOrDefault();
				if (item is ITagPair tagPair)
				{
					if (tagPair.StartTagProperties?.Formatting == null)
					{
						continue;
					}

					if (!tagPair.StartTagProperties.MetaDataContainsKey(CadfBackgroundColorKey))
					{
						continue;
					}

					segmentPair.Target.Clear();
					tagPair.MoveAllItemsTo(segmentPair.Target);

					document.UpdateSegmentPair(segmentPair);
				}
			}
		}

		private static int GetLargestSeedValue(IEnumerable<string> allTagIds)
		{
			var seed = 1;
			foreach (var id in allTagIds)
			{
				var success = int.TryParse(id, out var value);
				if (success)
				{
					if (value > seed)
					{
						seed = value;
					}
				}
			}

			return seed;
		}

		private static string GetColorRgb(Color color)
		{
			var colorRgb = "0, " + color.R + ", " + color.G + ", " + color.B;
			return colorRgb;
		}

		private static string GetColorName(Color color)
		{
			var name = color.Name;
			if (color.R == 102 && color.G == 255 && color.B == 0)
			{
				name = "brightGreen";
			}

			var colorName = name.Substring(0, 1);
			colorName = colorName.ToLower(CultureInfo.InvariantCulture);
			colorName += name.Substring(1);
			return colorName;
		}

		private static IEndTagProperties CreateEndTagProperties(IPropertiesFactory propertyFactory)
		{
			var endTagProperties = propertyFactory.CreateEndTagProperties("</cf>");
			endTagProperties.DisplayText = "cf";
			endTagProperties.CanHide = true;
			return endTagProperties;
		}

		private static IStartTagProperties CreateStartTagProperties(IPropertiesFactory propertyFactory,
			IFormattingItemFactory formattingFactory, IFormattingItem formattingItem, int tagId, string colorName)
		{
			var startTagProperties = propertyFactory.CreateStartTagProperties("<cf highlight=" + colorName + ">");
			if (startTagProperties.Formatting == null)
			{
				startTagProperties.Formatting = formattingFactory.CreateFormatting();
			}

			startTagProperties.Formatting.Add(formattingItem);
			startTagProperties.TagId = new TagId(tagId.ToString());
			startTagProperties.CanHide = true;
			startTagProperties.DisplayText = "cf";
			startTagProperties.SetMetaData(CadfBackgroundColorKey, "True");
			foreach (var keyPair in GetBackgroundColorMetaData(colorName))
			{
				startTagProperties.SetMetaData(keyPair.Key, keyPair.Value);
			}

			return startTagProperties;
		}

		private static int GetNextTagId(ICollection<string> allTagIds, int seed)
		{
			var i = seed;
			while (allTagIds.Contains(i.ToString()))
			{
				i++;
			}

			allTagIds.Add(i.ToString());
			return i;
		}

		private static List<string> GetAllTagIds(Document document)
		{
			var allTagIds = new List<string>();
			foreach (var segmentPair in document.SegmentPairs)
			{
				var paragraphUnit = ColorPickerHelper.GetParagraphUnit(segmentPair);
				if (paragraphUnit != null)
				{
					AddTagIds(paragraphUnit.Source, ref allTagIds);
					AddTagIds(paragraphUnit.Target, ref allTagIds);
				}

				AddTagIds(segmentPair.Source, ref allTagIds);
				AddTagIds(segmentPair.Target, ref allTagIds);
			}

			return allTagIds;
		}

		private static void AddTagIds(IAbstractMarkupDataContainer container, ref List<string> allTagIds)
		{
			foreach (var markupData in container.AllSubItems)
			{
				if (markupData is ITagPair tagPair)
				{
					if (tagPair.StartTagProperties != null)
					{
						var tagId = tagPair.StartTagProperties.TagId.Id;
						if (!allTagIds.Contains(tagId))
						{
							allTagIds.Add(tagId);
						}
					}
				}

				if (markupData is IPlaceholderTag placeholderTag)
				{
					if (placeholderTag.Properties != null)
					{
						var tagId = placeholderTag.Properties.TagId.Id;
						if (!allTagIds.Contains(tagId))
						{
							allTagIds.Add(tagId);
						}
					}
				}

				if (markupData is IAbstractTag abstractTag)
				{
					if (abstractTag.TagProperties != null)
					{
						var tagId = abstractTag.TagProperties.TagId.Id;
						if (!allTagIds.Contains(tagId))
						{
							allTagIds.Add(tagId);
						}
					}
				}
			}
		}

		private static Dictionary<string, string> GetBackgroundColorMetaData(string colorName)
		{
			var dictionary = new Dictionary<string, string>
			{
				{"ContentFormattingTagPairTypeKey", "True"},
				{"BackgroundColor", colorName},
				{"StartTag", "w:rPr"},
				{"ParentTag", "w:r"},
				{"w:highlight", $"<w:highlight w:val=\"{colorName}\" />"},
				{"Parent: w:highlight", "w:r"}
			};

			return dictionary;
		}
	}
}

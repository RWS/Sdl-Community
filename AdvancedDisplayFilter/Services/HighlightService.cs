using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sdl.Community.AdvancedDisplayFilter.Helpers;
using Sdl.Community.AdvancedDisplayFilter.Models;
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

		public void ApplyHighlighting(Document document, HighlightScrope highlightScrope, HighlightColor highlightColor)
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
						
			var itemFactory = document.ItemFactory;
			var propertyFactory = itemFactory.PropertiesFactory;
			var formattingFactory = propertyFactory.FormattingItemFactory;
			var formattingItem = formattingFactory.CreateFormattingItem("BackgroundColor", highlightColor.GetArgb());

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

				var startTagProperties = CreateStartTagProperties(propertyFactory, formattingFactory, formattingItem, tagId, highlightColor.Name);
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

		public List<HighlightColor> GetDefaultHighlightColors()
		{
			var colors = new List<HighlightColor>
			{
				new HighlightColor(Color.Yellow, "Yellow", PluginResources.square_yellow),
				new HighlightColor(Color.FromArgb(0, 102, 255, 0), "Bright Green", PluginResources.square_brightGreen),
				new HighlightColor(Color.Turquoise, "Turquoise", PluginResources.square_turquoise),
				new HighlightColor(Color.Pink, "Pink", PluginResources.square_pink),
				new HighlightColor(Color.Blue, "Blue", PluginResources.square_blue),
				new HighlightColor(Color.Red, "Red", PluginResources.square_red),
				new HighlightColor(Color.DarkBlue, "Dark Blue", PluginResources.square_darkBlue),
				new HighlightColor(Color.Teal, "Teal", PluginResources.square_teal),
				new HighlightColor(Color.Green, "Green", PluginResources.square_green),
				new HighlightColor(Color.Violet, "Violet", PluginResources.square_violet),
				new HighlightColor(Color.DarkRed, "Dark Red", PluginResources.square_darkRed)
			};

			return colors;
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

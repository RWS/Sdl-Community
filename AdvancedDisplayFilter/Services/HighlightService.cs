using System;
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
		private readonly List<HighlightColor> _highlightColors;

		public enum HighlightScope
		{
			Filtered,
			Active
		}

		public HighlightService()
		{
			_highlightColors = GetHighlightColors();
		}

		public void ApplyHighlightColor(Document document, HighlightScope highlightScope, HighlightColor highlightColor)
		{
			var segments = highlightScope == HighlightScope.Filtered
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
				var hashighlightColor = HasHighlightColor(segmentPair, out var existingHighlightColor);
				if (hashighlightColor)
				{
					if (existingHighlightColor != null &&
					    string.Equals(existingHighlightColor.Name, highlightColor.Name, StringComparison.InvariantCultureIgnoreCase))
					{
						continue;										
					}

					RemoveHighlightColor(segmentPair);
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

		public void ClearHighlightColors(Document document, HighlightScope highlightScope)
		{
			var segments = highlightScope == HighlightScope.Filtered
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

		public List<HighlightColor> GetHighlightColors()
		{
			var colors = new List<HighlightColor>
			{
				new HighlightColor(Color.FromArgb(0, 255, 255, 0), "Yellow", "yellow", PluginResources.rounded_yellow),
				new HighlightColor(Color.FromArgb(0, 0, 255, 0), "Green", "green", PluginResources.rounded_green),
				new HighlightColor(Color.FromArgb(0, 0, 255, 255), "Cyan", "cyan", PluginResources.rounded_cyan),
				new HighlightColor(Color.FromArgb(0, 255, 0, 255), "Magenta", "magenta", PluginResources.rounded_magenta),
				new HighlightColor(Color.FromArgb(0, 0, 0, 255), "Blue", "blue", PluginResources.rounded_blue),
				new HighlightColor(Color.FromArgb(0, 255, 0, 0), "Red", "red", PluginResources.rounded_red),
				new HighlightColor(Color.FromArgb(0, 0, 0, 128), "Dark Blue", "darkBlue", PluginResources.rounded_darkBlue),
				new HighlightColor(Color.FromArgb(0, 0, 128, 128), "Dark Cyan", "darkCyan", PluginResources.rounded_darkCyan),
				new HighlightColor(Color.FromArgb(0, 0, 128, 0), "Dark Green", "darkGreen", PluginResources.rounded_darkGreen),
				new HighlightColor(Color.FromArgb(0, 128, 0, 128), "Dark Magenta", "darkMagenta", PluginResources.rounded_darkMagenta),
				new HighlightColor(Color.FromArgb(0, 128, 0, 0), "Dark Red", "darkRed", PluginResources.rounded_darkRed),
				new HighlightColor(Color.FromArgb(0, 128, 128, 0), "Dark Yellow", "darkYellow", PluginResources.rounded_darkYellow),
				new HighlightColor(Color.FromArgb(0, 128, 128, 128), "Dark Gray", "darkGray", PluginResources.rounded_darkGray),
				new HighlightColor(Color.FromArgb(0, 192, 192, 192), "Light Gray", "lightGray", PluginResources.rounded_lightGray),
				new HighlightColor(Color.FromArgb(0, 0, 0, 0), "Black", "black", PluginResources.rounded_black)
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

		private bool HasHighlightColor(ISegmentPair segmentPair, out HighlightColor highlightColor)
		{
			highlightColor = null;

			var item = segmentPair.Target.AllSubItems.FirstOrDefault();
			if (item is ITagPair tagPair)
			{
				if (tagPair.StartTagProperties.Formatting != null &&
					tagPair.StartTagProperties.Formatting.ContainsKey("BackgroundColor") &&
					tagPair.StartTagProperties != null &&
					tagPair.StartTagProperties.MetaDataContainsKey(CadfBackgroundColorKey))
				{
					var argbValue = tagPair.StartTagProperties.Formatting.FirstOrDefault(a => a.Key == "BackgroundColor").Value;
					highlightColor = GetHighlightColorFromArgb(argbValue.StringValue);

					return true;
				}
			}

			return false;
		}

		private static void RemoveHighlightColor(ISegmentPair segmentPair)
		{
			var item = segmentPair.Target.AllSubItems.FirstOrDefault();
			if (item is ITagPair tagPair)
			{
				if (tagPair.StartTagProperties?.Formatting != null &&
					tagPair.StartTagProperties.MetaDataContainsKey(CadfBackgroundColorKey))
				{
					segmentPair.Target.Clear();
					tagPair.MoveAllItemsTo(segmentPair.Target);					
				}
			}
		}

		private HighlightColor GetHighlightColorFromArgb(string argbValue)
		{
			foreach (var highlightColor in _highlightColors)
			{
				var argb = highlightColor.GetArgb();
				if (string.Compare(argb, argbValue, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return highlightColor;
				}
			}

			return null;
		}
	}
}

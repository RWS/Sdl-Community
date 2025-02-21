using System;
using System.Collections.Generic;
using System.Linq;
using Multilingual.XML.FileType.Models;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;

namespace Multilingual.XML.FileType.Services
{
	public class QuickTagBuilder : AbstractQuickTagBuilder
	{
		private static readonly Lazy<FormattingItemFactory> LazyFormattingItemFactory = new Lazy<FormattingItemFactory>(() => new FormattingItemFactory());

		private static FormattingItemFactory InstanceFormattingItemFactory => LazyFormattingItemFactory.Value;


		public static List<TagMap> BuildStandardQuickTags()
		{
			var builder = new QuickTagBuilder();
			return builder.CreateStandardQuickTags();
		}

		internal List<TagMap> CreateStandardQuickTags()
		{
			var quickTags = new List<TagMap>();

			var tagMap = new TagMap();
			tagMap.QuickId = QuickTagDefaultId.Bold;
			tagMap.QuickInsertId = QuickInsertIds.qBold;
			tagMap.TagPair = CreateTagPair("<b>", "</b>", "b", new FormattingGroup { { "Bold", InstanceFormattingItemFactory.CreateFormattingItem("Bold", "true") } });
			tagMap.QuickTag = CreateDefaultTagPair(QuickTagDefaultId.Bold, "<b>", "</b>", "b");
			quickTags.Add(tagMap);

			tagMap = new TagMap();
			tagMap.QuickId = QuickTagDefaultId.Italic;
			tagMap.QuickInsertId = QuickInsertIds.qItalic;
			tagMap.TagPair = CreateTagPair("<i>", "</i>", "i", new FormattingGroup { { "Italic", InstanceFormattingItemFactory.CreateFormattingItem("Italic", "true") } });
			tagMap.QuickTag = CreateDefaultTagPair(QuickTagDefaultId.Italic, "<i>", "</i>", "i");
			quickTags.Add(tagMap);

			tagMap = new TagMap();
			tagMap.QuickId = QuickTagDefaultId.Italic;
			tagMap.QuickInsertId = QuickInsertIds.qItalic;
			tagMap.TagPair = CreateTagPair("<u>", "</u>", "u", new FormattingGroup { { "Underline", InstanceFormattingItemFactory.CreateFormattingItem("Underline", "true") } });
			tagMap.QuickTag = CreateDefaultTagPair(QuickTagDefaultId.Underline, "<u>", "</u>", "u");
			quickTags.Add(tagMap);

			tagMap = new TagMap();
			tagMap.QuickId = QuickTagDefaultId.Subscript;
			tagMap.QuickInsertId = QuickInsertIds.qSubscript;
			tagMap.TagPair = CreateTagPair("<sub>", "</sub>", "sub", new FormattingGroup { { "Subscript", InstanceFormattingItemFactory.CreateFormattingItem("Subscript", "true") } });
			tagMap.QuickTag = CreateDefaultTagPair(QuickTagDefaultId.Subscript, "<sub>", "</sub>", "sub");
			quickTags.Add(tagMap);

			tagMap = new TagMap();
			tagMap.QuickId = QuickTagDefaultId.Superscript;
			tagMap.QuickInsertId = QuickInsertIds.qSuperscript;
			tagMap.TagPair = CreateTagPair("<sup>", "</sup>", "sup", new FormattingGroup { { "Superscript", InstanceFormattingItemFactory.CreateFormattingItem("Superscript", "true") } });
			tagMap.QuickTag = CreateDefaultTagPair(QuickTagDefaultId.Superscript, "<sup>", "</sup>", "sup");
			quickTags.Add(tagMap);
	

			var bidiTags = CreateDefaultBidiQuickTags();

			foreach (var tag in bidiTags)
			{
				var existingTag = quickTags.FirstOrDefault(a => a.QuickTag.CommandId == tag.CommandId);
				if (existingTag == null)
				{
					quickTags.Add(new TagMap
					{
						QuickTag = tag
					});
				}
			}

			return quickTags;
		}

		private TagPair CreateTagPair(string startTagContent, string endTagContent, string displayText, IFormattingGroup formatting = null)
		{
			var startTagProperties = PropertiesFactory.CreateStartTagProperties(startTagContent);
			startTagProperties.Formatting = formatting;
			startTagProperties.CanHide = true;
			startTagProperties.DisplayText = displayText;
			startTagProperties.IsSoftBreak = false;
			startTagProperties.IsWordStop = false;

			var endTagProperties = PropertiesFactory.CreateEndTagProperties(endTagContent);
			endTagProperties.CanHide = startTagProperties.CanHide;
			endTagProperties.DisplayText = startTagProperties.DisplayText;
			endTagProperties.IsSoftBreak = startTagProperties.IsSoftBreak;
			endTagProperties.IsWordStop = startTagProperties.IsWordStop;

			return new TagPair(startTagProperties, endTagProperties);
		}
	}
}

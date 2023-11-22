using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdl.Community.StudioViews.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.StudioViews.Services
{
	public class SegmentBuilder
	{
		public SegmentBuilder()
		{
			ItemFactory = DefaultDocumentItemFactory.CreateInstance();
			PropertiesFactory = DefaultPropertiesFactory.CreateInstance();
			FormattingFactory = PropertiesFactory.FormattingItemFactory;
		}

		public IDocumentItemFactory ItemFactory { get; }

		public IPropertiesFactory PropertiesFactory { get; }

		public IFormattingItemFactory FormattingFactory { get; }

		public ITranslationOrigin CreateTranslationOrigin()
		{
			return ItemFactory.CreateTranslationOrigin();
		}

		public IParagraphUnit CreateParagraphUnit(IParagraphUnitProperties properties)
		{
			var paragraphUnit = ItemFactory.CreateParagraphUnit(properties?.LockType ?? LockTypeFlags.Unlocked);
			if (properties != null)
			{
				paragraphUnit.Properties = properties.Clone() as IParagraphUnitProperties;
			}

			return paragraphUnit;
		}

		public ISegmentPair CreateSegmentPair(ISegment source, ISegment target)
		{
			return ItemFactory.CreateSegmentPair(source, target);
		}

		public ISegment CreateSegment(ISegmentPairProperties segmentPairProperties)
		{
			return ItemFactory.CreateSegment(segmentPairProperties);
		}

		public IContextInfo CreateContextInfo(ParagraphUnitContext context)
		{
			var contextInfo = PropertiesFactory.CreateContextInfo(context.ContextType);
			contextInfo.DisplayName = context.DisplayName;
			contextInfo.DisplayCode = context.DisplayCode;
			contextInfo.Description = context.Description;
			foreach (var metaData in context.MetaData)
			{
				contextInfo.SetMetaData(metaData.Key, metaData.Value);
			}

			return contextInfo;
		}

		public IAbstractMarkupData CreatePlaceholder(string tagId, string displayText, string tagContent, ref List<string> existingTagIds)
		{
			var textProperties = PropertiesFactory.CreatePlaceholderTagProperties(tagContent);
			if (!string.IsNullOrEmpty(displayText))
			{
				textProperties.DisplayText = displayText;
			}

			textProperties.SetMetaData("localName", "ph");
			//textProperties.SetMetaData("displayText", displayText);
			textProperties.SetMetaData("attribute:id", tagId);


			if (existingTagIds.Contains(textProperties.TagId.Id))
			{
				textProperties.TagId = !existingTagIds.Contains(tagId)
					? new TagId(tagId)
					: new TagId(GetUniqueTagPairId(existingTagIds));
			}

			if (!existingTagIds.Contains(textProperties.TagId.Id))
			{
				existingTagIds.Add(textProperties.TagId.Id);
			}

			return ItemFactory.CreatePlaceholderTag(textProperties);
		}

		public IAbstractMarkupData Text(string text)
		{
			var textProperties = PropertiesFactory.CreateTextProperties(text);
			return ItemFactory.CreateText(textProperties);
		}

		public ISegmentPairProperties CreateSegmentPairProperties()
		{
			var properties = ItemFactory.CreateSegmentPairProperties();
			properties.TranslationOrigin = CreateTranslationOrigin();
			return properties;
		}

		public IComment CreateComment(string text, string author, Severity severity, DateTime dateTime, string version)
		{
			var comment = PropertiesFactory.CreateComment(text, author, severity);
			comment.Date = dateTime;
			comment.Version = version;
			return comment;
		}

		public IAbstractMarkupData CreateCommentContainer(string text, string author, Severity severity, DateTime dateTime, string version)
		{
			var comment = PropertiesFactory.CreateComment(text, author, severity);
			comment.Date = dateTime;
			comment.Version = version;

			var commentProperties = PropertiesFactory.CreateCommentProperties();
			commentProperties.Add(comment);
			var commentMarker = ItemFactory.CreateCommentMarker(commentProperties);

			return commentMarker;
		}

		public ITagPair CreateTagPair(string tagId, string tagContent, ref List<string> existingTagIds)
		{
			var tagName = GetStartTagName(tagContent, out var refId);
			var startTagProperties = PropertiesFactory.CreateStartTagProperties("<" + tagName + " id=\"" + tagId + "\">");
			startTagProperties.DisplayText = tagContent;
			startTagProperties.SetMetaData("localName", tagName);
			startTagProperties.SetMetaData("displayText", tagContent);
			startTagProperties.SetMetaData("attribute:id", tagId);

			if (existingTagIds.Contains(startTagProperties.TagId.Id))
			{
				startTagProperties.TagId = !existingTagIds.Contains(tagId)
					? new TagId(tagId)
					: new TagId(GetUniqueTagPairId(existingTagIds));
			}

			if (!existingTagIds.Contains(startTagProperties.TagId.Id))
			{
				existingTagIds.Add(startTagProperties.TagId.Id);
			}

			var endTagProperties = PropertiesFactory.CreateEndTagProperties("</" + tagName + ">");
			endTagProperties.DisplayText = "</" + tagName + ">";
			endTagProperties.SetMetaData("localName", tagName);
			endTagProperties.SetMetaData("displayText", "</" + tagName + ">");
			endTagProperties.SetMetaData("attribute:id", tagId);


			//TODO formatting example
			//var xItem = _formattingFactory.CreateFormattingItem("italic", "True");
			//x.Formatting = _formattingFactory.CreateFormatting();
			//x.Formatting.Add(xItem);

			var tagPair = ItemFactory.CreateTagPair(startTagProperties, endTagProperties);


			return tagPair;
		}

		private string GetUniqueTagPairId(List<string> existingTagIds)
		{
			var id = 1;
			while (existingTagIds.Contains(id.ToString()))
			{
				id++;
			}

			return id.ToString();
		}

		public IAbstractMarkupData CreateLockedContent()
		{
			var lockedContentProperties = PropertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual);
			var lockedContent = ItemFactory.CreateLockedContent(lockedContentProperties);
			return lockedContent;
		}


		public string GetStartTagName(string text, out string refId)
		{
			var tagName = string.Empty;
			refId = string.Empty;
			var regexTagName = new Regex(@"\<(?<name>[^\s""\>]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			var regexAttribute = new Regex(@"\s+(?<name>[^\s""]+)\=""(?<value>[^""]*)""", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			var m = regexTagName.Match(text);
			if (m.Success)
			{
				tagName = m.Groups["name"].Value;
			}

			var mc = regexAttribute.Matches(text);
			if (mc.Count > 0)
			{
				foreach (Match match in mc)
				{
					var attValue = match.Groups["value"].Value;
					var attName = match.Groups["name"].Value;

					if (string.Compare(attName, "id", StringComparison.OrdinalIgnoreCase) == 0)
					{
						refId = attValue;
					}
				}
			}

			return tagName;
		}

		public string GetEndTagName(string text)
		{
			var tagName = string.Empty;

			var regexTagName = new Regex(@"\<\s*\/\s*(?<name>[^\s\>]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			var m = regexTagName.Match(text);
			if (m.Success)
			{
				tagName = m.Groups["name"].Value;
			}

			return tagName;
		}

		public string GetEmptyTagName(string text)
		{
			var tagName = string.Empty;

			var regexTagName = new Regex(@"\<\s*(?<name>[^\s\>\/]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			var m = regexTagName.Match(text);
			if (m.Success)
			{
				tagName = m.Groups["name"].Value;
			}

			return tagName;
		}
	}
}

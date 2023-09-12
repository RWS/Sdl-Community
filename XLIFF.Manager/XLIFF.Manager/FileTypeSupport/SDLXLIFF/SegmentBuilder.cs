using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	public class SegmentBuilder
	{
		private readonly IDocumentItemFactory _factory;
		private readonly IPropertiesFactory _propertiesFactory;
		private readonly IFormattingItemFactory _formattingFactory;

		public SegmentBuilder()
		{
			ItemFactory = DefaultDocumentItemFactory.CreateInstance();
			PropertiesFactory = DefaultPropertiesFactory.CreateInstance();
			FormattingFactory = PropertiesFactory.FormattingItemFactory;
		}

		public IDocumentItemFactory ItemFactory { get; }

		public IPropertiesFactory PropertiesFactory { get; }

		public IFormattingItemFactory FormattingFactory { get; }

		public List<string> ExistingTagIds { get; set; }

		public ITranslationOrigin CreateTranslationOrigin()
		{
			return ItemFactory.CreateTranslationOrigin();
		}

		public IContextInfo CreateContextInfo(Context context)
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

		public IAbstractMarkupData CreatePlaceholder(string tagId, string displayText, string tagContent)
		{
			var textProperties = PropertiesFactory.CreatePlaceholderTagProperties(tagContent);
			if (!string.IsNullOrEmpty(displayText))
			{
				textProperties.DisplayText = displayText;
			}

			textProperties.SetMetaData("localName", "ph");
			//textProperties.SetMetaData("displayText", displayText);
			textProperties.SetMetaData("attribute:id", tagId);

			if (ExistingTagIds.Contains(textProperties.TagId.Id))
			{
				textProperties.TagId = !ExistingTagIds.Contains(tagId)
					? new TagId(tagId)
					: new TagId(GetUniqueTagPairId());
			}

			if (!ExistingTagIds.Contains(textProperties.TagId.Id))
			{
				ExistingTagIds.Add(textProperties.TagId.Id);
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

		public IAbstractMarkupData CreateTagPair(string tagId, string tagContent)
		{
			var tagName = GetStartTagName(tagContent, out var refId);

			// Dev Notes: the tagContent is switched with the Display text to align with how the tags are 
			// recreated by the XLIFF 1.2 parser from the framework

			var startTagProperties = PropertiesFactory.CreateStartTagProperties("<bpt id=\"" + tagId + "\">");
			startTagProperties.DisplayText = tagContent;
			startTagProperties.SetMetaData("localName", "bpt");
			startTagProperties.SetMetaData("displayText", tagContent);
			startTagProperties.SetMetaData("attribute:id", tagId);

			if (ExistingTagIds.Contains(startTagProperties.TagId.Id))
			{
				startTagProperties.TagId = !ExistingTagIds.Contains(tagId)
					? new TagId(tagId)
					: new TagId(GetUniqueTagPairId());
			}

			if (!ExistingTagIds.Contains(startTagProperties.TagId.Id))
			{
				ExistingTagIds.Add(startTagProperties.TagId.Id);
			}

			var endTagProperties = PropertiesFactory.CreateEndTagProperties("<ept id=\"" + tagId + "\">");
			endTagProperties.DisplayText = "</" + tagName + ">";
			endTagProperties.SetMetaData("localName", "ept");
			endTagProperties.SetMetaData("displayText", "</" + tagName + ">");
			endTagProperties.SetMetaData("attribute:id", tagId);


			//TODO formatting example
			//var xItem = _formattingFactory.CreateFormattingItem("italic", "True");
			//x.Formatting = _formattingFactory.CreateFormatting();
			//x.Formatting.Add(xItem);

			var tagPair = ItemFactory.CreateTagPair(startTagProperties, endTagProperties);

			return tagPair;
		}

		private string GetUniqueTagPairId()
		{
			var id = 1;
			while (ExistingTagIds.Contains(id.ToString()))
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

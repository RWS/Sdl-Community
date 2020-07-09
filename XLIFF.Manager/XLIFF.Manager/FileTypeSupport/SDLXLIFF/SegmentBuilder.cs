using System;
using System.Text.RegularExpressions;
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
			_factory = DefaultDocumentItemFactory.CreateInstance();
			_propertiesFactory = DefaultPropertiesFactory.CreateInstance();
			_formattingFactory = _propertiesFactory.FormattingItemFactory;
		}

		public ITranslationOrigin CreateTranslationOrigin()
		{
			return _factory.CreateTranslationOrigin();
		}

		public IAbstractMarkupData CreatePlaceholder(string tagId, string text)
		{
			var textProperties = _propertiesFactory.CreatePlaceholderTagProperties(text);
			//if (!string.IsNullOrEmpty(tagId))
			//{
			//	textProperties.TagId = new TagId(tagId);
			//}

			return _factory.CreatePlaceholderTag(textProperties);
		}

		public IAbstractMarkupData Text(string text)
		{
			var textProperties = _propertiesFactory.CreateTextProperties(text);
			return _factory.CreateText(textProperties);
		}

		public ISegmentPairProperties CreateSegmentPairProperties()
		{
			var properties = _factory.CreateSegmentPairProperties();
			properties.TranslationOrigin = CreateTranslationOrigin();
			return properties;
		}

		public IComment CreateComment(string text, string author, Severity severity, DateTime dateTime, string version)
		{			
			var comment = _propertiesFactory.CreateComment(text, author, severity);
			comment.Date = dateTime;
			comment.Version = version;
			return comment;
		}

		public IAbstractMarkupData CreateCommentContainer(string text, string author, Severity severity, DateTime dateTime, string version)
		{			
			var comment = _propertiesFactory.CreateComment(text, author, severity);
			comment.Date = dateTime;
			comment.Version = version;

			var commentProperties = _propertiesFactory.CreateCommentProperties();
			commentProperties.Add(comment);
			var commentMarker = _factory.CreateCommentMarker(commentProperties);

			return commentMarker;
		}

		public IAbstractMarkupData CreateTagPair(string tagId, string tagContent)
		{
			var tagName = GetStartTagName(tagContent, out var refId);

			var startTagProperties = _propertiesFactory.CreateStartTagProperties(tagContent);
			//if (!string.IsNullOrEmpty(tagId))
			//{
			//	startTagProperties.TagId = new TagId(tagId);
			//}

			var endTagProperties = _propertiesFactory.CreateEndTagProperties("</" + tagName + ">");

			//TODO formatting example
			//var xItem = _formattingFactory.CreateFormattingItem("italic", "True");
			//x.Formatting = _formattingFactory.CreateFormatting();
			//x.Formatting.Add(xItem);

			var tagPair = _factory.CreateTagPair(startTagProperties, endTagProperties);
			
			return tagPair;
		}

		public IAbstractMarkupData CreateLockedContent()
		{
			var lockedContentProperties = _propertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual);			
			var lockedContent = _factory.CreateLockedContent(lockedContentProperties);			
			return lockedContent;
		}


		public string GetStartTagName(string text, out string refId)
		{
			var tagName = string.Empty;
			refId = string.Empty;
			var regexTagName = new Regex(@"\<(?<name>[^\s""\>]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			var regexTagId = new Regex(@"\<[^\s""]*\s+(?<name>[^\s""]+)\=""(?<value>[^""]*)""", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			var m = regexTagName.Match(text);
			if (m.Success)
			{
				tagName = m.Groups["name"].Value;
			}

			m = regexTagId.Match(text);
			if (m.Success)
			{
				var id = m.Groups["value"].Value;
				var attName = m.Groups["name"].Value;

				if (string.Compare(attName, "id", StringComparison.OrdinalIgnoreCase) == 0)
				{
					refId = id;
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

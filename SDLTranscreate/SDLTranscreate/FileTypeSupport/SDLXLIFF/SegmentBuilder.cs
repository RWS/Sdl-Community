using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Trados.Transcreate.FileTypeSupport.MSOffice.Model;
using Trados.Transcreate.FileTypeSupport.XLIFF.Model;

namespace Trados.Transcreate.FileTypeSupport.SDLXLIFF
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

		public IAbstractMarkupData CreatePlaceholder(string tagId, string tagContent)
		{
			// Dev Notes: the tagContent is switched with the Display text to align with how the tags are 
			// recreated by the XLIFF 1.2 parser from the framework

			var textProperties = PropertiesFactory.CreatePlaceholderTagProperties("<ph id=\"" + tagId + "\"/>");
			textProperties.DisplayText = tagContent;
			textProperties.SetMetaData("localName", "ph");
			textProperties.SetMetaData("displayText", tagContent);
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



		public ISegment GetUpdatedSegment(ISegment targetSegment, IEnumerable<Token> tokens, ISegment sourceSegment)
		{
			var vector = new List<int>();
			var lockedContentId = 0;

			//store original segment
			var originalSegment = (ISegment)targetSegment.Clone();

			//remove old content
			targetSegment.Clear();

			foreach (var item in tokens)
			{
				switch (item.Type)
				{
					case Token.TokenType.TagOpen:
						var tagPairOpen = (IAbstractMarkupDataContainer)GetElement(
							GetTagID(item.Content), originalSegment, sourceSegment, item.Type);
						tagPairOpen.Clear();
						InsertItemOnLocation(vector, ref targetSegment,
							(IAbstractMarkupData)tagPairOpen);
						vector.Add(((ITagPair)tagPairOpen).IndexInParent);
						break;
					case Token.TokenType.TagClose:
						vector.RemoveAt(vector.Count - 1);
						break;
					case Token.TokenType.TagPlaceholder:
						InsertItemOnLocation(vector, ref targetSegment,
							GetElement(GetTagID(item.Content), originalSegment, sourceSegment,
								item.Type));
						break;
					case Token.TokenType.Text:
						InsertItemOnLocation(vector, ref targetSegment, ItemFactory.CreateText(
							PropertiesFactory.CreateTextProperties(item.Content)));
						break;
					case Token.TokenType.LockedContent:
						InsertItemOnLocation(vector, ref targetSegment,
							GetElement(lockedContentId.ToString(), originalSegment, sourceSegment,
								item.Type));
						lockedContentId++;
						break;
					case Token.TokenType.CommentStart:
						var commentContainer = GetComment(item);
						InsertItemOnLocation(vector, ref targetSegment,
							(IAbstractMarkupData)commentContainer);
						vector.Add(((ICommentMarker)commentContainer).IndexInParent);
						break;
					case Token.TokenType.CommentEnd:
						if (vector.Count > 0)
						{
							vector.RemoveAt(vector.Count - 1);
						}

						break;
					case Token.TokenType.RevisionMarker:
						//hasTrackedChanges = true;
						if (item.RevisionType == Token.RevisionMarkerType.InsertStart)
						{
							var insertContainer = GetRevisionMarker(item, RevisionType.Insert);
							InsertItemOnLocation(vector, ref targetSegment,
								(IAbstractMarkupData)insertContainer);
							vector.Add(((IRevisionMarker)insertContainer).IndexInParent);
						}
						else if (item.RevisionType == Token.RevisionMarkerType.DeleteStart)
						{
							var insertContainer = GetRevisionMarker(item, RevisionType.Delete);
							InsertItemOnLocation(vector, ref targetSegment,
								(IAbstractMarkupData)insertContainer);
							vector.Add(((IRevisionMarker)insertContainer).IndexInParent);
						}
						else
						{
							if (vector.Count > 0)
							{
								vector.RemoveAt(vector.Count - 1);
							}
						}

						break;
				}
			}

			return targetSegment;
		}

		/// <summary>
		/// Insert item to the proper container
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="segment"></param>
		/// <param name="abstractItem"></param>
		private void InsertItemOnLocation(IEnumerable<int> vector, ref ISegment segment, IAbstractMarkupData abstractItem)
		{
			IAbstractMarkupDataContainer currentContainer = segment;

			foreach (var index in vector)
			{
				currentContainer = (IAbstractMarkupDataContainer)currentContainer[index];
			}

			currentContainer.Add(abstractItem);
		}

		/// <summary>
		/// Create IRevisionMarker from specified Token
		/// </summary>
		/// <param name="item"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private IAbstractMarkupDataContainer GetRevisionMarker(Token item, RevisionType type)
		{
			var revisionProperties = ItemFactory.CreateRevisionProperties(type);
			revisionProperties.Author = item.Author;
			revisionProperties.Date = item.Date;
			var revision = ItemFactory.CreateRevision(revisionProperties);
			return revision;
		}

		/// <summary>
		/// Create ICommentMarker from specified Token
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private IAbstractMarkupDataContainer GetComment(Token item)
		{
			var commentProperties = PropertiesFactory.CreateCommentProperties();
			var comment = PropertiesFactory.CreateComment(item.Content, item.Author, Severity.Low);
			comment.Date = item.Date;
			commentProperties.Add(comment);
			var commentMarker = ItemFactory.CreateCommentMarker(commentProperties);
			return commentMarker;
		}

		/// <summary>
		/// Get the MarkupData ID
		/// </summary>
		/// <param name="tagContent"></param>
		/// <returns></returns>
		private string GetTagID(string tagContent)
		{
			return tagContent.Replace("<", "").Replace(">", "").Replace("/", "");
		}

		/// <summary>
		/// Get the required MarkupData from the original segment, if the tag doesn't exist in original target, source segment will be searched as well.
		/// </summary>
		/// <param name="tagId"></param>
		/// <param name="originalTargetSegment"></param>
		/// <param name="sourceSegment"></param>
		/// <param name="tokenType"></param>
		/// <returns></returns>
		private IAbstractMarkupData GetElement(string tagId, ISegment originalTargetSegment, ISegment sourceSegment, Token.TokenType tokenType)
		{
			var extractor = new MSOffice.Visitors.ElementExtractor();
			extractor.GetTag(tagId, originalTargetSegment, tokenType);
			if (extractor.FoundElement != null)
			{
				return (IAbstractMarkupData)extractor.FoundElement.Clone();
			}

			//tag not found in original target, try to search in source
			extractor.GetTag(tagId, sourceSegment, tokenType);
			if (extractor.FoundElement != null)
			{
				return (IAbstractMarkupData)extractor.FoundElement.Clone();
			}

			if (tokenType == Token.TokenType.TagOpen || tokenType == Token.TokenType.TagClose || tokenType == Token.TokenType.TagPlaceholder)
			{
				throw new Exception("Tags in segment ID " + originalTargetSegment.Properties.Id.Id + " are corrupted!");
			}
			if (tokenType == Token.TokenType.LockedContent)
			{
				throw new Exception("Locked contents in segment ID " + originalTargetSegment.Properties.Id.Id + " are corrupted!");
			}

			throw new Exception("Problem when reading segment #" + originalTargetSegment.Properties.Id.Id);
		}

	}
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	internal class SegmentVisitor : IMarkupDataVisitor
	{
		private readonly bool _ignoreTags;
		private readonly Regex _regexNewline = new Regex(@"[\r\n]+", RegexOptions.None);

		private Stack<ITagPair> _tagPairStack;

		public SegmentVisitor(bool ignoreTags = false)
		{
			_ignoreTags = ignoreTags;
			InitializeComponents();
		}

		public List<Element> Elements { get; private set; }

		public bool HasRevisions { get; private set; }

		public Dictionary<string, List<IComment>> Comments { get; set; }

		public string Text { get; private set; }

		private void InitializeComponents()
		{
			_tagPairStack = new Stack<ITagPair>();
			Elements = new List<Element>();
			Text = string.Empty;
			Comments = new Dictionary<string, List<IComment>>();
		}

		private void VisitChilderen(IAbstractMarkupDataContainer container)
		{
			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (!_ignoreTags)
			{
				var element = new ElementTagPair
				{
					Type = Element.TagType.OpeningTag,
					TagId = tagPair.TagProperties.TagId.Id,
					TagContent = tagPair.TagProperties.TagContent,
					DisplayText = tagPair.TagProperties.DisplayText
				};

				Elements.Add(element);

				Text += tagPair.StartTagProperties.TagContent;
				_tagPairStack.Push(tagPair);
			}

			VisitChilderen(tagPair);

			if (!_ignoreTags)
			{
				var currentTag = _tagPairStack.Pop();

				var element = new ElementTagPair
				{
					Type = Element.TagType.ClosingTag,
					TagId = currentTag.TagProperties.TagId.Id,
					TagContent = currentTag.EndTagProperties.TagContent
				};

				Elements.Add(element);

				Text += currentTag.EndTagProperties.TagContent;
			}
		}

		public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
		{
			if (_ignoreTags)
			{
				return;
			}

			var element = new ElementPlaceholder
			{
				TagId = placeholderTag.TagProperties.TagId.Id,
				TagContent = placeholderTag.TagProperties.TagContent,
				DisplayText = placeholderTag.TagProperties.DisplayText,
				TextEquivalent = placeholderTag.Properties.TextEquivalent
			};

			Elements.Add(element);

			Text += placeholderTag.Properties.TagContent;
		}

		public void VisitSegment(ISegment segment)
		{
			InitializeComponents();
			VisitChilderen(segment);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			// not used in this implementation
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			var id = Guid.NewGuid().ToString();
			var comments = new List<IComment>();
			foreach (var comment in commentMarker.Comments.Comments)
			{
				comments.Add(comment);
				//The severity level has not been specified
				//Undefined = 0,

				//Informational purpose
				//Low = 1,

				//Warning, likely an important issue
				//Medium = 2,

				//Error, a severe issue
				//High = 3,

				//Sentinel, not used
				//Invalid = 100
			}

			Comments.Add(id, comments);

			var commentOpen = new ElementComment
			{
				Type = Element.TagType.OpeningTag,
				Id = id
			};
			Elements.Add(commentOpen);

			VisitChilderen(commentMarker);

			var commentClose = new ElementComment
			{
				Type = Element.TagType.ClosingTag,
				Id = id
			};
			Elements.Add(commentClose);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChilderen(marker);
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			if (_ignoreTags)
			{
				return;
			}

			var element = new ElementLocked
			{
				Type = Element.TagType.OpeningTag
			};

			Elements.Add(element);

			VisitChilderen(lockedContent.Content);

			element = new ElementLocked
			{
				Type = Element.TagType.ClosingTag
			};

			Elements.Add(element);
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			HasRevisions = true;

			if (revisionMarker.Properties.RevisionType == RevisionType.Insert
				|| revisionMarker.Properties.RevisionType == RevisionType.FeedbackAdded)
			{
				VisitChilderen(revisionMarker);
			}
		}
		
		public void VisitText(IText text)
		{
			var elements = GetInlineText(text);
			if (elements.Count > 0)
			{
				Elements.AddRange(elements);
			}

			Text += text.Properties.Text;
		}

		private List<Element> GetInlineText(IText text)
		{
			var elements = new List<Element>();

			var matches = _regexNewline.Matches(text.Properties.Text);
			if (matches.Count > 0)
			{
				var startIndex = 0;
				foreach (Match match in matches)
				{
					var prefix = text.Properties.Text.Substring(startIndex, match.Index - startIndex);
					if (!string.IsNullOrEmpty(prefix))
					{
						elements.Add(new ElementText { Text = prefix });
					}

					var lbText = text.Properties.Text.Substring(match.Index, match.Length);
					elements.Add(new ElementGenericPlaceholder
					{
						CType = "lb",
						TextEquivalent = lbText.Replace("\n", "\\n").Replace("\r", "\\r")
					});

					startIndex = match.Index + match.Length;
				}

				if (startIndex < text.Properties.Text.Length)
				{
					var suffixText = text.Properties.Text.Substring(startIndex);
					elements.Add(new ElementText { Text = suffixText });
				}
			}
			else
			{
				elements.Add(new ElementText { Text = text.Properties.Text });
			}

			return elements;
		}
	}
}

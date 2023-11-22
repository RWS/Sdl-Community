using System;
using System.Collections.Generic;
using System.Linq;
using Multilingual.XML.FileType.Models;
using Multilingual.XML.FileType.Services.Entities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Services
{
	public class SegmentVisitor : IMarkupDataVisitor
	{
		private Stack<ITagPair> _tagPairStack;

		private EntityService _entityService;

		private bool _reader;

		public SegmentVisitor(EntityService entityService, bool reader, bool ignoreTags = false)
		{
			_entityService = entityService;
			_reader = reader;
			IgnoreTags = ignoreTags;
			InitializeComponents();
		}

		public List<ElementTagPair> TagPairElements { get; private set; }

		public List<ElementPlaceholder> PlaceholderElements { get; private set; }

		public List<Element> Elements { get; private set; }

		public bool HasRevisions { get; private set; }

		public bool IgnoreTags { get; set; }

		public Dictionary<string, List<IComment>> Comments { get; set; }

		public string Text { get; private set; }

		public void InitializeComponents()
		{
			_tagPairStack = new Stack<ITagPair>();
			Elements = new List<Element>();
			TagPairElements = new List<ElementTagPair>();
			PlaceholderElements = new List<ElementPlaceholder>();
			Text = string.Empty;
			Comments = new Dictionary<string, List<IComment>>();
		}

		private void VisitChilderen(IAbstractMarkupDataContainer container)
		{
			if (container == null)
			{
				return;
			}

			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (!IgnoreTags)
			{
				var element = new ElementTagPair
				{
					Type = Element.TagType.TagOpen,
					TagId = tagPair.TagProperties.TagId.Id,
					TagContent = tagPair.TagProperties.TagContent,
					DisplayText = tagPair.TagProperties.DisplayText
				};

				if (!TagPairElements.Exists(a => a.TagId == element.TagId))
				{
					TagPairElements.Add(element);
				}

				Elements.Add(element);

				Text += tagPair.StartTagProperties.TagContent;
				_tagPairStack.Push(tagPair);
			}

			VisitChilderen(tagPair);

			if (!IgnoreTags)
			{
				var currentTag = _tagPairStack.Pop();

				var element = new ElementTagPair
				{
					Type = Element.TagType.TagClose,
					TagId = currentTag.TagProperties.TagId.Id,
					TagContent = currentTag.EndTagProperties.TagContent
				};

				Elements.Add(element);

				Text += currentTag.EndTagProperties.TagContent;
			}
		}

		public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
		{
			if (IgnoreTags)
			{
				return;
			}

			var convertedEntity = _entityService.ConvertEntityPlaceholderToText(placeholderTag.Properties.TagContent);

			var element = new ElementPlaceholder
			{
				TagId = placeholderTag.TagProperties.TagId.Id,
				TagContent = convertedEntity,
				DisplayText = placeholderTag.TagProperties.DisplayText,
				TextEquivalent = placeholderTag.Properties.TextEquivalent
			};

			if (!PlaceholderElements.Exists(a => a.TagId == element.TagId))
			{
				PlaceholderElements.Add(element);
			}

			Elements.Add(element);

			Text += convertedEntity;
		}

		public void VisitText(IText text)
		{
			var escapedText = text.Properties.Text;

			if (_reader)
			{
				escapedText = _entityService.ConvertKnownEntitiesInText(escapedText, EntityRule.Parser);
			}
			else
			{
				escapedText = _entityService.ConvertKnownEntitiesInText(escapedText, EntityRule.Writer);
				escapedText = _entityService.ConvertKnownCharactersToEntities(escapedText);
			}

			var lastElement = Elements.LastOrDefault();
			if (lastElement is ElementText textElement)
			{
				textElement.Text += escapedText;
			}
			else
			{
				Elements.Add(new ElementText
				{
					Text = escapedText
				});
			}

			Text += escapedText;
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
				Type = Element.TagType.TagOpen,
				Id = id
			};
			Elements.Add(commentOpen);

			VisitChilderen(commentMarker);

			var commentClose = new ElementComment
			{
				Type = Element.TagType.TagClose,
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
			if (IgnoreTags)
			{
				return;
			}

			var element = new ElementLocked
			{
				Type = Element.TagType.TagOpen
			};

			Elements.Add(element);

			VisitChilderen(lockedContent.Content);

			element = new ElementLocked
			{
				Type = Element.TagType.TagClose
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
	}
}

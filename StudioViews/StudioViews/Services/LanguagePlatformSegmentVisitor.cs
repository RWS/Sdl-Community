using System.Collections.Generic;
using System.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.StudioViews.Services
{
	internal class LanguagePlatformSegmentVisitor : IMarkupDataVisitor
	{
		private readonly bool _ignoreTags;

		private readonly Stack<Tag> _tagPairStack;

		private int _anchor;

		public LanguagePlatformSegmentVisitor(CultureInfo culture, bool ignoreTags)
		{
			Segment = new Segment(culture);
			_ignoreTags = ignoreTags;
			_anchor = 0;
			Comments = new List<IComment>();
			_tagPairStack = new Stack<Tag>();
		}

		public bool HasRevisions { get; private set; }

		public Segment Segment { get; }

		public List<IComment> Comments { get; }

		private void VisitChilderen(IAbstractMarkupDataContainer container)
		{
			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			_anchor++;
			var tagId = tagPair.StartTagProperties.TagId.Id;

			if (!_ignoreTags)
			{
				var tag = new Tag(
					TagType.Start,
					tagId,
					_anchor,
					0,
					null,
					tagPair.StartTagProperties.CanHide);

				Segment?.Add(tag);

				_tagPairStack.Push(tag);
			}

			VisitChilderen(tagPair);

			if (!_ignoreTags)
			{
				var currentTag = _tagPairStack.Pop();

				var tag = new Tag(
					TagType.End,
					currentTag.TagID,
					currentTag.Anchor,
					0,
					null,
					currentTag.CanHide);
				Segment?.Add(tag);
			}

		}

		public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
		{

			if (_ignoreTags)
				return;

			_anchor++;
			var tag = new Tag(
				TagType.Standalone, placeholderTag.Properties.TagId.Id, _anchor);
			if (placeholderTag.Properties.HasTextEquivalent)
			{
				tag.Type = TagType.TextPlaceholder;
				tag.TextEquivalent = placeholderTag.Properties.TextEquivalent;
			}
			Segment?.Add(tag);
		}

		public void VisitText(IText text)
		{
			Segment?.Add(text.Properties.Text);
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChilderen(segment);

		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			// not used in this implementation
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			foreach (var commentProperty in commentMarker.Comments.Comments)
			{
				Comments.Add(commentProperty);
			}
			VisitChilderen(commentMarker);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChilderen(marker);
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			if (_ignoreTags)
				return;

			_anchor++;
			var tag = new Tag(
				TagType.LockedContent, _anchor.ToString(), _anchor)
			{
				TextEquivalent = lockedContent.Content.ToString()
			};
			Segment?.Add(tag);
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

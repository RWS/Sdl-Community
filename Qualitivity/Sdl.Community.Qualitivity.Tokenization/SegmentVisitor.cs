using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.Tokenization
{
	internal class SegmentVisitor : IMarkupDataVisitor
	{
		private bool IgnoreTags { get; }
		private int TagId { get; set; }

		public bool HasRevisions { get; private set; }
		public Segment Segment { get; }
		public List<IComment> Comments { get; set; }

		public SegmentVisitor(Segment segment, bool ignoreTags)
		{
			Segment = segment;
			IgnoreTags = ignoreTags;
			TagId = 0;
			Comments = new List<IComment>();
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

			TagId++;
			var tagId = tagPair.StartTagProperties.TagId.Id;

			if (!IgnoreTags)
			{
				var tag = new Tag(
					TagType.Start,
					tagId,
					TagId,
					0,
					null,
					tagPair.StartTagProperties.CanHide);

				Segment?.Add(tag);
			}

			VisitChilderen(tagPair);

			if (!IgnoreTags)
			{
				var tag = new Tag(
					TagType.End,
					tagId,
					TagId,
					0,
					null,
					tagPair.EndTagProperties.CanHide);
				Segment?.Add(tag);
			}

		}

		public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
		{

			if (IgnoreTags)
				return;

			TagId++;
			var tag = new Tag(
				TagType.Standalone, placeholderTag.Properties.TagId.Id, TagId);
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
			if (IgnoreTags)
				return;

			TagId++;
			var tag = new Tag(
				TagType.LockedContent, TagId.ToString(), TagId)
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

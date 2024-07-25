using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.StudioViews.Services
{
	public class SegmentGenerator : IMarkupDataVisitor
	{

		public Segment Segment { get; set; }


		public void ProcessSegment(ISegment segment, bool includeTagText, List<string> revisionMarkersUniqueIds)
		{
			Segment = new Segment();

			VisitChildren(segment);
		}


		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			if (container == null)
				return;

			foreach (var item in container)
				item.AcceptVisitor(this);
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			VisitChildren(commentMarker);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			// Not required for this implementation.
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			var objTag = new Tag
			{
				TextEquivalent = lockedContent.Content.ToString(),
				Type = TagType.LockedContent
			};


			Segment.Add(objTag);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{

			VisitChildren(marker);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{

			var placeHolderTag = new Tag
			{
				Type = TagType.TextPlaceholder,
				TagID = tag.TagProperties.TagId.Id,
				TextEquivalent = tag.Properties.HasTextEquivalent ? tag.Properties.TextEquivalent : tag.Properties.DisplayText
			};

			Segment.Add(placeHolderTag);

		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			VisitChildren(revisionMarker);
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitTagPair(ITagPair tagPair)
		{

			var tgStart = new Tag
			{
				Type = TagType.Start,
				TagID = tagPair.StartTagProperties.TagId.Id,
				TextEquivalent = tagPair.StartTagProperties.DisplayText
			};
			Segment.Add(tgStart);


			VisitChildren(tagPair);


			var tgEnd = new Tag
			{
				Type = TagType.End,
				TagID = tagPair.StartTagProperties.TagId.Id,
				TextEquivalent = tagPair.EndTagProperties.DisplayText
			};
			Segment.Add(tgEnd);

		}

		public void VisitText(IText text)
		{
			Segment.Add(text.Properties.Text);
		}
	}
}

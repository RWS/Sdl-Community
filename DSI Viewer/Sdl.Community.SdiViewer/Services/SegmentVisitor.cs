using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.DsiViewer.Services
{
	public class SegmentVisitor : IMarkupDataVisitor
	{
		private readonly bool _ignoreTags;

		private Stack<ITagPair> _tagPairStack;

		public SegmentVisitor(bool ignoreTags)
		{
			_ignoreTags = ignoreTags;
			InitializeComponents();
		}

		public bool HasRevisions { get; private set; }

		public List<IComment> Comments { get; set; }

		public string Text { get; private set; }

		private void InitializeComponents()
		{
			_tagPairStack = new Stack<ITagPair>();
			Text = string.Empty;
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
			if (!_ignoreTags)
			{
				Text += tagPair.StartTagProperties.TagContent;
				_tagPairStack.Push(tagPair);
			}

			VisitChilderen(tagPair);

			if (!_ignoreTags)
			{
				var currentTag = _tagPairStack.Pop();
				Text += currentTag.EndTagProperties.TagContent;
			}
		}

		public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
		{
			if (_ignoreTags)
			{
				return;
			}

			Text += placeholderTag.Properties.TagContent;
		}

		public void VisitText(IText text)
		{
			Text += text.Properties.Text;
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
			{
				return;
			}

			Text += lockedContent.Content.ToString();
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

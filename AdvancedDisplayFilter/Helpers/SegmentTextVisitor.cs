using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public class SegmentTextVisitor : IMarkupDataVisitor
	{
		private enum DetailLevel
		{
			JustText,
			Raw,
			JustTagContent
		}

		private readonly StringBuilder _textBuilder = new StringBuilder();
		private DetailLevel _detailLevel;

		public string GetText(ISegment segment)
		{
			_textBuilder.Clear();
			_detailLevel = DetailLevel.JustText;
			VisitChildren(segment);

			return _textBuilder.ToString();
		}

		public string GetRawText(ISegment segment)
		{
			_textBuilder.Clear();
			_detailLevel = DetailLevel.Raw;
			return GetText(segment);
		}

		public string GetJustTagContent(ISegment segment)
		{
			_textBuilder.Clear();
			_detailLevel = DetailLevel.JustTagContent;
			return GetText(segment);
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		/// <summary>
		/// Check if tag pair contains specified property
		/// </summary>
		/// <returns></returns>
		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			if (_detailLevel == DetailLevel.JustTagContent)
			{
				_textBuilder.Append(tag.TagProperties.TagContent);
			}
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (_detailLevel == DetailLevel.JustText)
			{
				VisitChildren(tagPair);
			}
			else if (_detailLevel == DetailLevel.JustTagContent)
			{
				_textBuilder.Append(tagPair.TagProperties.TagContent);
				VisitChildren(tagPair);
			}
		}

		public void VisitText(IText text)
		{
			if (_detailLevel == DetailLevel.JustText)
			{
				_textBuilder.Append(text.Properties.Text);
			}
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			if (container == null)
				return;
			if (_detailLevel == DetailLevel.Raw)
			{
				_textBuilder.Append(container);
			}
			else
			{
				foreach (var item in container)
				{
					item.AcceptVisitor(this);
				}
			}
		}
	}
}
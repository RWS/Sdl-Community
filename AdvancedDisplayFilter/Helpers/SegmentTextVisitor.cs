using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.AdvancedDisplayFilter.Helpers
{
	public class SegmentTextVisitor : IMarkupDataVisitor
	{
		public enum DetailLevel
		{
			JustText,
			Raw,
			JustTagContent
		}

		private readonly StringBuilder _textBuilder;
		private DetailLevel _detailLevel;

		public SegmentTextVisitor()
		{
			_textBuilder = new StringBuilder();
			_detailLevel = DetailLevel.JustText;
		}

		public string GetText(ISegment segment, DetailLevel detailLevel = DetailLevel.JustText)
		{
			_textBuilder.Clear();
			_detailLevel = detailLevel;

			VisitChildren(segment);

			return _textBuilder.ToString();
		}


		public void VisitCommentMarker(ICommentMarker commentMarker)
		{			
			VisitChildren(commentMarker);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			// ignore; not used in this implementation
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			// ignore; not used in this implementation
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
			// ignore; not used in this implementation
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			switch (_detailLevel)
			{
				case DetailLevel.JustText:
					{
						VisitChildren(tagPair);
					}
					break;
				case DetailLevel.JustTagContent:
					{
						_textBuilder.Append(tagPair.TagProperties.TagContent);

						VisitChildren(tagPair);
					}
					break;
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
			{
				return;
			}

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
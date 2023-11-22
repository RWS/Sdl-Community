using System.Collections.Generic;
using System.Linq;
using Multilingual.Excel.FileType.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Services
{
	public class SegmentTagAligner : IMarkupDataVisitor
	{

		private readonly ElementTags _sourceDocumentTags;

		private readonly List<string> _tagIds;
		private ElementTags _sourceSegmentTags;

		public SegmentTagAligner(ElementTags sourceDocumentTags)
		{
			_sourceDocumentTags = sourceDocumentTags;

			_tagIds = _sourceDocumentTags.PlaceholderElements.Select(a => a.TagId).ToList();
			foreach (var tagId in _sourceDocumentTags.TagPairElements.Select(a => a.TagId))
			{
				if (!_tagIds.Contains(tagId))
				{
					_tagIds.Add(tagId);
				}
			}
		}

		public void NormalizeTags(ISegment segment, ElementTags sourceSegmentTags)
		{
			_sourceSegmentTags = sourceSegmentTags;
			VisitSegment(segment);
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
			var element = new ElementTagPair
			{
				Type = Element.TagType.TagOpen,
				TagId = tagPair.TagProperties.TagId.Id,
				TagContent = tagPair.TagProperties.TagContent,
				DisplayText = tagPair.TagProperties.DisplayText
			};

			var segmentTagMatch = _sourceSegmentTags.TagPairElements.FirstOrDefault(a => a.TagContent == element.TagContent);
			if (segmentTagMatch != null)
			{
				element.TagId = segmentTagMatch.TagId;
				tagPair.StartTagProperties.TagId = new TagId(element.TagId);

				_sourceSegmentTags.TagPairElements.Remove(segmentTagMatch);
			}
			else
			{
				var documentTagPairMatch = _sourceDocumentTags.TagPairElements.FirstOrDefault(a => a.TagId == element.TagId);
				if (documentTagPairMatch != null)
				{
					if (documentTagPairMatch.TagContent == element.TagContent)
					{
						element.TagId = documentTagPairMatch.TagId;
						tagPair.StartTagProperties.TagId = new TagId(element.TagId);
					}
					else
					{
						element.TagId = GetUniqueTagId();
						tagPair.StartTagProperties.TagId = new TagId(element.TagId);
					}
				}
				else if (_sourceDocumentTags.PlaceholderElements.Exists(a => a.TagId == element.TagId))
				{
					element.TagId = GetUniqueTagId();
					tagPair.StartTagProperties.TagId = new TagId(element.TagId);
				}
			}

			if (!_sourceDocumentTags.TagPairElements.Exists(a => a.TagId == element.TagId))
			{
				_sourceDocumentTags.TagPairElements.Add(element);
			}

			VisitChilderen(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
		{
			var element = new ElementPlaceholder
			{
				TagId = placeholderTag.TagProperties.TagId.Id,
				TagContent = placeholderTag.TagProperties.TagContent,
				DisplayText = placeholderTag.TagProperties.DisplayText,
				TextEquivalent = placeholderTag.Properties.TextEquivalent
			};

			var segmentTagMatch = _sourceSegmentTags.PlaceholderElements.FirstOrDefault(a => a.TagContent == element.TagContent);
			if (segmentTagMatch != null)
			{
				element.TagId = segmentTagMatch.TagId;
				placeholderTag.Properties.TagId = new TagId(element.TagId);

				_sourceSegmentTags.PlaceholderElements.Remove(segmentTagMatch);
			}
			else
			{
				var documentPlaceholderTagMatch = _sourceDocumentTags.PlaceholderElements.FirstOrDefault(a => a.TagId == element.TagId);
				if (documentPlaceholderTagMatch != null)
				{
					if (documentPlaceholderTagMatch.TagContent == element.TagContent)
					{
						element.TagId = documentPlaceholderTagMatch.TagId;
						placeholderTag.Properties.TagId = new TagId(element.TagId);
					}
					else
					{
						element.TagId = GetUniqueTagId();
						placeholderTag.Properties.TagId = new TagId(element.TagId);
					}
				}
				else if (_sourceDocumentTags.TagPairElements.Exists(a => a.TagId == element.TagId))
				{
					element.TagId = GetUniqueTagId();
					placeholderTag.Properties.TagId = new TagId(element.TagId);
				}
			}

			if (!_sourceDocumentTags.PlaceholderElements.Exists(a => a.TagId == element.TagId))
			{
				_sourceDocumentTags.PlaceholderElements.Add(element);
			}
		}

		private string GetUniqueTagId()
		{
			var id = 1;
			while (_tagIds.Contains(id.ToString()))
			{
				id++;
			}

			_tagIds.Add(id.ToString());

			return id.ToString();
		}

		public void VisitText(IText text)
		{
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
			VisitChilderen(commentMarker);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChilderen(marker);
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			VisitChilderen(lockedContent.Content);
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			if (revisionMarker.Properties.RevisionType == RevisionType.Insert
				|| revisionMarker.Properties.RevisionType == RevisionType.FeedbackAdded)
			{
				VisitChilderen(revisionMarker);
			}
		}
	}
}

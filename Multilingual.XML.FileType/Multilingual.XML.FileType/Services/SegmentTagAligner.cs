using System.Collections.Generic;
using System.Linq;
using Multilingual.XML.FileType.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Services
{
	public class SegmentTagAligner : IMarkupDataVisitor
	{
		private readonly ElementTags _sourceDocumentTags;

		public List<string> TagIds { get; set; }
		//private ElementTags _sourceSegmentTags;

		public SegmentTagAligner(ElementTags sourceDocumentSourceTags, ElementTags sourceDocumentTargetTags)
		{
			_sourceDocumentTags = sourceDocumentSourceTags;
			foreach (var placeholder in sourceDocumentTargetTags.PlaceholderElements.Where(placeholder
						 => !_sourceDocumentTags.PlaceholderElements.Exists(a => a.TagId == placeholder.TagId)))
			{
				_sourceDocumentTags.PlaceholderElements.Add(placeholder);
			}

			foreach (var tagPair in sourceDocumentTargetTags.TagPairElements.Where(tagPair
						 => !_sourceDocumentTags.TagPairElements.Exists(a => a.TagId == tagPair.TagId)))
			{
				_sourceDocumentTags.TagPairElements.Add(tagPair);
			}

			TagIds = _sourceDocumentTags.PlaceholderElements.Select(a => a.TagId).ToList();
			foreach (var tagId in _sourceDocumentTags.TagPairElements.Select(a => a.TagId))
			{
				if (!TagIds.Contains(tagId))
				{
					TagIds.Add(tagId);
				}
			}
		}

		public void NormalizeTags(ISegment segment, ElementTags sourceSegmentTags)
		{
			//_sourceSegmentTags = sourceSegmentTags;
			VisitSegment(segment);
		}

		public void NormalizeTags(IParagraph paragraph)
		{
			foreach (var abstractMarkupData in paragraph.AllSubItems)
			{
				switch (abstractMarkupData)
				{
					case IPlaceholderTag placeholderTag:
					{
						VisitPlaceholderTag(placeholderTag);

						//var element = new ElementPlaceholder
						//{
						//	TagId = placeholderTag.TagProperties.TagId.Id,
						//	TagContent = placeholderTag.TagProperties.TagContent,
						//	DisplayText = placeholderTag.TagProperties.DisplayText,
						//	TextEquivalent = placeholderTag.Properties.TextEquivalent
						//};

							//element.TagId = GetUniqueTagId();
							//placeholderTag.Properties.TagId = new TagId(element.TagId);

							//if (!_sourceDocumentTags.PlaceholderElements.Exists(a => a.TagId == element.TagId))
							//{
							//	_sourceDocumentTags.PlaceholderElements.Add(element);
							//}

							break;
					}

					case ITagPair tagPair:
					{
						VisitTagPair(tagPair);
						//var element = new ElementTagPair
						//{
						//	Type = Element.TagType.TagOpen,
						//	TagId = tagPair.TagProperties.TagId.Id,
						//	TagContent = tagPair.TagProperties.TagContent,
						//	DisplayText = tagPair.TagProperties.DisplayText
						//};

						//element.TagId = GetUniqueTagId();
						//tagPair.StartTagProperties.TagId = new TagId(element.TagId);


						//if (!_sourceDocumentTags.TagPairElements.Exists(a => a.TagId == element.TagId))
						//{
						//	_sourceDocumentTags.TagPairElements.Add(element);
						//}

						break;
					}
				}
			}
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

			element.TagId = GetUniqueTagId(element.TagId);
			tagPair.StartTagProperties.TagId = new TagId(element.TagId);

			//var segmentTagMatch = _sourceSegmentTags.TagPairElements.FirstOrDefault(a => a.TagContent == element.TagContent);
			//if (segmentTagMatch != null)
			//{
			//	element.TagId = segmentTagMatch.TagId;
			//	tagPair.StartTagProperties.TagId = new TagId(element.TagId);

			//	_sourceSegmentTags.TagPairElements.Remove(segmentTagMatch);
			//}
			//else
			//{


			//	var documentTagPairMatch = _sourceDocumentTags.TagPairElements.FirstOrDefault(a => a.TagId == element.TagId);
			//	if (documentTagPairMatch != null)
			//	{
			//		if (documentTagPairMatch.TagContent == element.TagContent)
			//		{
			//			element.TagId = documentTagPairMatch.TagId;
			//			tagPair.StartTagProperties.TagId = new TagId(element.TagId);
			//		}
			//		else
			//		{
			//			element.TagId = GetUniqueTagId();
			//			tagPair.StartTagProperties.TagId = new TagId(element.TagId);
			//		}
			//	}
			//	else if (_sourceDocumentTags.PlaceholderElements.Exists(a => a.TagId == element.TagId))
			//	{
			//		element.TagId = GetUniqueTagId();
			//		tagPair.StartTagProperties.TagId = new TagId(element.TagId);
			//	}
			//}

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

			element.TagId = GetUniqueTagId(element.TagId);
			placeholderTag.Properties.TagId = new TagId(element.TagId);

			//var segmentTagMatch = _sourceSegmentTags.PlaceholderElements.FirstOrDefault(a => a.TagContent == element.TagContent);
			//if (segmentTagMatch != null)
			//{
			//	element.TagId = segmentTagMatch.TagId;
			//	placeholderTag.Properties.TagId = new TagId(element.TagId);

			//	_sourceSegmentTags.PlaceholderElements.Remove(segmentTagMatch);
			//}
			//else
			//{
			//	var documentPlaceholderTagMatch = _sourceDocumentTags.PlaceholderElements.FirstOrDefault(a => a.TagId == element.TagId);
			//	if (documentPlaceholderTagMatch != null)
			//	{
			//		if (documentPlaceholderTagMatch.TagContent == element.TagContent)
			//		{
			//			element.TagId = documentPlaceholderTagMatch.TagId;
			//			placeholderTag.Properties.TagId = new TagId(element.TagId);
			//		}
			//		else
			//		{
			//			element.TagId = GetUniqueTagId();
			//			placeholderTag.Properties.TagId = new TagId(element.TagId);
			//		}
			//	}
			//	else if (_sourceDocumentTags.TagPairElements.Exists(a => a.TagId == element.TagId))
			//	{
			//		element.TagId = GetUniqueTagId();
			//		placeholderTag.Properties.TagId = new TagId(element.TagId);
			//	}
			//}

			if (!_sourceDocumentTags.PlaceholderElements.Exists(a => a.TagId == element.TagId))
			{
				_sourceDocumentTags.PlaceholderElements.Add(element);
			}
		}

		private string GetUniqueTagId(string tagId)
		{
			if (!TagIds.Exists(a=>a == tagId))
			{
				TagIds.Add(tagId);
				return tagId;
			}

			var id = 1;
			while (TagIds.Contains(id.ToString()))
			{
				id++;
			}

			TagIds.Add(id.ToString());

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

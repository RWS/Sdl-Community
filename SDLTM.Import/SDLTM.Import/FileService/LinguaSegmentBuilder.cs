using System;
using System.Collections.Generic;
using System.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;

namespace SDLTM.Import.FileService
{
	public class LinguaSegmentBuilder : IMarkupDataVisitor
	{
		private readonly bool _excludeTagsInLockedContentText;
		private int _nextAnchor;
		private readonly bool _acceptTrackChanges;
		private readonly bool _includeTrackChanges;
		private readonly bool _includeComments;
		private readonly Func<IRevisionMarker, string> _revisionMarkerTagIdFunction;

		public bool HasTrackChanges { get; private set; }
		/// <summary>
		/// The lingua segment all content will be appended to.
		/// </summary>
		/// <remarks>Must be set before using the builder.</remarks>
		public Segment Result { get; set; }

		/// <summary>
		/// Contains tag relationships between Core.Tag's and BCM Tags after segment conversion. Not all 
		/// Core.Tag's necessarily have associated BCM tags, in which case the value part is null.
		/// </summary>
		public List<KeyValuePair<Tag, IAbstractMarkupData>> TagAssociations { get; }


		/// <summary>
		/// Contains text relationships between Core.Text's and BCM Texts after segment conversion. Not all 
		/// Core.Text's necessarily have associated BCM text, in which case the value part is null.
		/// </summary>
		public List<KeyValuePair<Text, IAbstractMarkupData>> TextAssociations { get; }

		/// <summary>
		/// When <c>true</c> tags are not included in the result.
		/// </summary>
		public bool IgnoreTags { get; set; }


		/// <summary>
		/// Construct with a segment instance that will receive the results.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="flags"></param>
		public LinguaSegmentBuilder(Segment result, LinguaTuBuilderSettings flags)
		{
			Result = result;

			IgnoreTags = flags.StripTags;
			_excludeTagsInLockedContentText = flags.ExcludeTagsInLockedContentText;
			_acceptTrackChanges = flags.AcceptTrackChanges;
			_includeTrackChanges = flags.IncludeTrackChanges;
			_includeComments = flags.IncludeComments;
			_revisionMarkerTagIdFunction = flags.RevisionMarkerTagIdFunction;

			_nextAnchor = 0;
			TagAssociations = new List<KeyValuePair<Tag, IAbstractMarkupData>>();
			TextAssociations = new List<KeyValuePair<Text, IAbstractMarkupData>>();

		}

		/// <inheritdoc />
		/// <summary>
		/// Construct with a segment instance that will receive the results.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="ignoreTags"></param>
		/// <param name="excludeTagsInLockedContentText">when true any locked content will be represented as
		/// lingua locked content tags with a text equivalent that does not include tag content (typically used
		/// when updating the TM). When false locked content rag will have a text equivalent containing all tag and text content that has
		/// been locked (this is useful to determine which locked content parts match between source and target, or
		/// between the document and the TM match)</param>
		public LinguaSegmentBuilder(Segment result, bool ignoreTags, bool excludeTagsInLockedContentText)
			: this(result, new LinguaTuBuilderSettings
			{
				StripTags = ignoreTags,
				ExcludeTagsInLockedContentText = excludeTagsInLockedContentText,
				AcceptTrackChanges = true
			})
		{
		}

		/// <inheritdoc />
		/// <summary>
		/// Construct with a segment instance that will receive the results.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="ignoreTags"></param>
		/// <param name="excludeTagsInLockedContentText"></param>
		/// <param name="acceptTrackChanges">accept or reject track changes</param>
		/// <param name="includeTrackChanges"> </param>
		public LinguaSegmentBuilder(Segment result, bool ignoreTags, bool excludeTagsInLockedContentText,
			bool acceptTrackChanges, bool includeTrackChanges = false)
			: this(result, new LinguaTuBuilderSettings
			{
				StripTags = ignoreTags,
				ExcludeTagsInLockedContentText = excludeTagsInLockedContentText,
				AcceptTrackChanges = acceptTrackChanges,
				IncludeTrackChanges = includeTrackChanges
			})
		{
		}

		/// <summary>
		/// Visit each child node in the container.
		/// </summary>
		/// <param name="container"></param>
		public void VisitChildNodes(IAbstractMarkupDataContainer container)
		{
			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}


		/// <summary>
		/// visits all comment content
		/// </summary>
		/// <param name="commentMarker"></param>
		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			++_nextAnchor;
			var anchor = _nextAnchor;
			var tagId = $"c{commentMarker.UniqueId}";

			if (_includeComments)
			{
				// add start tag
				var startTag = new Tag(TagType.Start,
					tagId, anchor,
					0, null, false);
				RecordTag(startTag, commentMarker);
			}
			// we only care about the child nodes
			VisitChildNodes(commentMarker);


			if (!_includeComments) return;
			// add end tag
			var endTag = new Tag(TagType.End, tagId, anchor, 0, null, false);
			RecordTag(endTag, commentMarker);
		}


		/// <summary>
		/// ignored
		/// </summary>
		/// <param name="location"></param>
		public void VisitLocationMarker(ILocationMarker location)
		{
			// can safely be ignored
		}

		/// <summary>
		/// Visits all content inside the locked content.
		/// </summary>
		/// <param name="lockedContent"></param>
		public void VisitLockedContent(ILockedContent lockedContent)
		{
			// locked content inside a segment is treated as special type of placeable.
			if (IgnoreTags) return;
			++_nextAnchor;

			var linguaTag = new Tag(TagType.LockedContent, _nextAnchor.ToString(), _nextAnchor)
			{
				TextEquivalent = _excludeTagsInLockedContentText
					? TextCollector.CollectText(lockedContent)
					: lockedContent.Content.ToString()
			};
			
			RecordTag(linguaTag, null);
		}


		/// <summary>
		/// Visits all content inside the marker.
		/// </summary>
		/// <param name="marker"></param>
		public void VisitOtherMarker(IOtherMarker marker)
		{
			// we are only interested in the child nodes
			VisitChildNodes(marker);
		}
		
		/// <summary>
		/// Append a standalone tag to the result.
		/// </summary>
		/// <param name="placeholderTag"></param>
		public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
		{
			if (IgnoreTags) return;
			// add standalone tag
			++_nextAnchor;

			var linguaTag = new Tag(
				TagType.Standalone,
				placeholderTag.TagProperties.TagId.Id, _nextAnchor);

			if (placeholderTag.Properties.HasTextEquivalent)
			{
				linguaTag.Type = TagType.TextPlaceholder;
				linguaTag.TextEquivalent = placeholderTag.Properties.TextEquivalent;
			}

			RecordTag(linguaTag, placeholderTag);
		}

		private void RecordTag(Tag tag, IAbstractMarkupData bcmTag)
		{
			Result.Add(tag);
			TagAssociations.Add(new KeyValuePair<Tag, IAbstractMarkupData>(tag, bcmTag));
		}

		/// <summary>
		/// Should never be called. Throws an exception.
		/// </summary>
		/// <param name="segment"></param>
		public void VisitSegment(ISegment segment)
		{
			// should never happen
			throw new LanguagePlatformException(ErrorCode.UnexpectedDocumentContent);
		}

		/// <summary>
		/// Appends a start tag to the result, visits all tag pair content and appends an end tag to the result.
		/// </summary>
		/// <param name="tagPair"></param>
		public void VisitTagPair(ITagPair tagPair)
		{
			++_nextAnchor;
			var anchor = _nextAnchor;
			var tagId = tagPair.StartTagProperties.TagId.Id;

			if (!IgnoreTags)
			{
				// add start tag
				var startTag = new Tag(TagType.Start, tagId, anchor, 0, null, tagPair.StartTagProperties.CanHide);
				RecordTag(startTag, tagPair);
			}

			// visit all tag pair content
			VisitChildNodes(tagPair);

			if (IgnoreTags) return;

			// add end tag
			var endTag = new Tag(TagType.End, tagId, anchor, 0, null, tagPair.EndTagProperties.CanHide);
			RecordTag(endTag, tagPair);
		}


		/// <summary>
		/// Appends text to the result.
		/// </summary>
		public void VisitText(IText bcmText)
		{
			// add text
			Result.Add(bcmText.Properties.Text);

			var linguaText = (Text)Result.Elements[Result.Elements.Count - 1];

			TextAssociations.Add(new KeyValuePair<Text, IAbstractMarkupData>(linguaText, bcmText));
		}

		/// <summary>
		/// Visits all child nodes in the revision marker.
		/// </summary>
		/// <param name="revisionMarker"></param>
		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			HasTrackChanges = true;
			if (_includeTrackChanges)
			{
				++_nextAnchor;
				var anchor = _nextAnchor;
				var tagId = string.Concat(revisionMarker.Properties.Author, '|',
					revisionMarker.Properties.RevisionType, '|',
					revisionMarker.Properties.Date != null
						? revisionMarker.Properties.Date.Value.ToString(CultureInfo.InvariantCulture)
						: string.Empty);

				if (_revisionMarkerTagIdFunction != null)
				{
					string temp = _revisionMarkerTagIdFunction(revisionMarker);
					if (!string.IsNullOrEmpty(temp))
						tagId = temp;
				}

				// add start tag
				var startTag = new Tag(TagType.Start, tagId, anchor);
				RecordTag(startTag, revisionMarker);

				// visit all tag pair content
				VisitChildNodes(revisionMarker);

				// add end tag
				var endTag = new Sdl.LanguagePlatform.Core.Tag(Sdl.LanguagePlatform.Core.TagType.End,
					tagId, anchor);
				RecordTag(endTag, revisionMarker);
			}
			else
			{
				// we are only interested in the child nodes of inserted content
				if (_acceptTrackChanges)
				{
					if (revisionMarker.Properties.RevisionType == RevisionType.Insert ||
					    revisionMarker.Properties.RevisionType == RevisionType.FeedbackAdded)
					{
						VisitChildNodes(revisionMarker);
					}
				}
				else
				{
					if (revisionMarker.Properties.RevisionType == RevisionType.Delete ||
					    revisionMarker.Properties.RevisionType == RevisionType.FeedbackDeleted)
					{
						VisitChildNodes(revisionMarker);
					}
				}

				if (revisionMarker.Properties.RevisionType == RevisionType.FeedbackComment ||
				    revisionMarker.Properties.RevisionType == RevisionType.Unchanged)
				{
					VisitChildNodes(revisionMarker);
				}
			}
		}
	}
}

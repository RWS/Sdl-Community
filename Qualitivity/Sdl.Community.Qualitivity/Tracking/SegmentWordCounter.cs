using System;
using System.Collections.Concurrent;
using System.Globalization;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.Qualitivity.Tracking
{
	// A TM search returns the source word counts even when nothing matches, so an empty in-memory TM counts like the toolkit's file-based one
	public class SegmentWordCounter
	{
		// In-memory TMs stay registered for the whole Studio session, so there is one per language pair
		private static readonly ConcurrentDictionary<string, ITranslationMemoryLanguageDirection> Memories =
			new ConcurrentDictionary<string, ITranslationMemoryLanguageDirection>();

		private static readonly SearchSettings ExactSearch = new SearchSettings
		{
			MaxResults = 1,
			MinScore = 100,
			Mode = SearchMode.ExactSearch,
			ComputeTranslationProposal = false
		};

		private readonly CultureInfo _sourceLanguage;
		private readonly ITranslationMemoryLanguageDirection _memory;

		public SegmentWordCounter(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			_sourceLanguage = sourceLanguage;
			_memory = Memories.GetOrAdd(sourceLanguage.Name + "|" + targetLanguage.Name, _ =>
				new InMemoryTranslationMemory("Qualitivity word count " + Guid.NewGuid(), new CultureCode(sourceLanguage.Name),
					new CultureCode(targetLanguage.Name), FuzzyIndexes.SourceWordBased, BuiltinRecognizers.RecognizeAll).LanguageDirection);
		}

		public WordCounts Count(ISegmentPair segmentPair)
		{
			var segment = new Segment(_sourceLanguage);
			new Visitor(segment).VisitSegment(segmentPair.Source);
			if (segment.Elements.Count == 0)
			{
				// Like the toolkit's TM lookup, an empty source is counted from its target
				new Visitor(segment).VisitSegment(segmentPair.Target);
			}
			return _memory.SearchSegment(ExactSearch, segment).SourceWordCounts;
		}
		private class Visitor : IMarkupDataVisitor
		{
			private readonly Segment _segment;
			private int _anchor;

			public Visitor(Segment segment)
			{
				_segment = segment;
			}

			public void VisitSegment(ISegment segment) => VisitChildren(segment);

			public void VisitText(IText text) => _segment.Add(text.Properties.Text);

			public void VisitTagPair(ITagPair tagPair)
			{
				var anchor = ++_anchor;
				var id = tagPair.StartTagProperties.TagId.Id;
				var canHide = tagPair.StartTagProperties.CanHide;
				_segment.Add(new Tag(TagType.Start, id, anchor, 0, null, canHide));
				VisitChildren(tagPair);
				_segment.Add(new Tag(TagType.End, id, anchor, 0, null, canHide));
			}

			public void VisitPlaceholderTag(IPlaceholderTag tag)
			{
				var element = new Tag(TagType.Standalone, tag.Properties.TagId.Id, ++_anchor);
				if (tag.Properties.HasTextEquivalent)
				{
					element.Type = TagType.TextPlaceholder;
					element.TextEquivalent = tag.Properties.TextEquivalent;
				}
				_segment.Add(element);
			}

			public void VisitLocationMarker(ILocationMarker location)
			{
			}

			public void VisitCommentMarker(ICommentMarker commentMarker) => VisitChildren(commentMarker);

			public void VisitOtherMarker(IOtherMarker marker) => VisitChildren(marker);

			public void VisitLockedContent(ILockedContent lockedContent)
			{
				var anchor = ++_anchor;
				_segment.Add(new Tag(TagType.LockedContent, anchor.ToString(), anchor)
				{
					TextEquivalent = lockedContent.Content.ToString()
				});
			}

			// Like the toolkit: only inserted (and feedback-added) text counts, deleted text does not
			public void VisitRevisionMarker(IRevisionMarker revisionMarker)
			{
				var type = revisionMarker.Properties.RevisionType;
				if (type == RevisionType.Insert || type == RevisionType.FeedbackAdded)
				{
					VisitChildren(revisionMarker);
				}
			}

			private void VisitChildren(IAbstractMarkupDataContainer container)
			{
				foreach (var item in container)
				{
					item.AcceptVisitor(this);
				}
			}
		}
	}
}

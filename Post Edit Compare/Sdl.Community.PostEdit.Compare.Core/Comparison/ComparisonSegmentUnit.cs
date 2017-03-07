using System.Collections.Generic;

namespace Sdl.Community.PostEdit.Compare.Core.Comparison
{
    public partial class Comparer
    {
        public class ComparisonSegmentUnit
        {
            public string SegmentId { get; set; }
            public List<ComparisonTextUnit> ComparisonTextUnits { get; set; }

            public List<SDLXLIFF.SegmentSection> Source { get; set; }

            public List<SDLXLIFF.SegmentSection> TargetOriginal { get; set; }
            public List<SDLXLIFF.SegmentSection> TargetUpdated { get; set; }

            public List<SDLXLIFF.SegmentSection> TargetOriginalRevisionMarkers { get; set; }
            public List<SDLXLIFF.SegmentSection> TargetUpdatedRevisionMarkers { get; set; }


            public string SegmentStatusOriginal { get; set; }
            public string SegmentStatusUpdated { get; set; }

            public string TranslationStatusOriginal { get; set; }
            public string TranslationStatusUpdated { get; set; }

            public string TranslationOriginTypeOriginal { get; set; }
            public string TranslationOriginTypeUpdated { get; set; }


            public int SourceWordsOriginal { get; set; }
            public int SourceCharsOriginal { get; set; }
            public int SourceTagsOriginal { get; set; }
            public int SourcePlaceablesOriginal { get; set; }

            public int SourceWordsUpdated { get; set; }
            public int SourceCharsUpdated { get; set; }
            public int SourceTagsUpdated { get; set; }
            public int SourcePlaceablesUpdated { get; set; }

            public bool SegmentIsLocked { get; set; }


            public List<SDLXLIFF.Comment> Comments { get; set; }

            public bool SegmentTextUpdated { get; set; }
            public bool SegmentSegmentStatusUpdated { get; set; }
            public bool SegmentHasComments { get; set; }



            public ComparisonSegmentUnit(string segmentId, List<SDLXLIFF.SegmentSection> source, List<SDLXLIFF.SegmentSection> targetOriginal, List<SDLXLIFF.SegmentSection> targetUpdated, bool segmentIsLocked)
            {
                SourceWordsOriginal = 0;
                SourceCharsOriginal = 0;
                SourceTagsOriginal = 0;
                SourcePlaceablesOriginal = 0;

                SourceWordsUpdated = 0;
                SourceCharsUpdated = 0;
                SourceTagsUpdated = 0;
                SourcePlaceablesUpdated = 0;

                SegmentId = segmentId;
                ComparisonTextUnits = new List<ComparisonTextUnit>();

                Source = source;
                TargetOriginal = targetOriginal;
                TargetUpdated = targetUpdated;

                SegmentStatusOriginal = string.Empty;
                SegmentStatusUpdated = string.Empty;

                TranslationStatusOriginal = string.Empty;
                TranslationStatusUpdated = string.Empty;

                TranslationOriginTypeOriginal = string.Empty;
                TranslationOriginTypeUpdated = string.Empty;

                Comments = null;

                SegmentTextUpdated = false;
                SegmentSegmentStatusUpdated = false;
                SegmentHasComments = false;

                SegmentIsLocked = segmentIsLocked;
            }
        }
    }
}

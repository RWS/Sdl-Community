using System.Collections.Generic;

namespace Sdl.Community.XliffCompare.Core.Comparer
{
    internal partial class Comparer
    {
        internal class ComparisonSegmentUnit
        {
            internal string SegmentId { get; set; }
            internal List<ComparisonTextUnit> ComparisonTextUnits { get; set; }

            internal List<SDLXLIFF.SegmentSection> Source { get; set; }

            internal List<SDLXLIFF.SegmentSection> TargetOriginal { get; set; }
            internal List<SDLXLIFF.SegmentSection> TargetUpdated { get; set; }

            internal string SegmentStatusOriginal { get; set; }
            internal string SegmentStatusUpdated { get; set; }

            internal string TranslationStatusOriginal { get; set; }
            internal string TranslationStatusUpdated { get; set; }

            internal string TranslationOriginTypeOriginal { get; set; }
            internal string TranslationOriginTypeUpdated { get; set; }




            internal decimal TranslationSectionsWords { get; set; }
            internal decimal TranslationSectionsCharacters { get; set; }
            internal decimal TranslationSectionsTags { get; set; }

            internal decimal TranslationSectionsWordsIdentical { get; set; }
            internal decimal TranslationSectionsCharactersIdentical { get; set; }
            internal decimal TranslationSectionsTagsIdentical { get; set; }

            internal decimal TranslationSectionsWordsNew { get; set; }
            internal decimal TranslationSectionsCharactersNew { get; set; }
            internal decimal TranslationSectionsTagsNew { get; set; }
                        
            internal decimal TranslationSectionsWordsRemoved { get; set; }
            internal decimal TranslationSectionsCharactersRemoved { get; set; }
            internal decimal TranslationSectionsTagsRemoved { get; set; } 

            internal decimal TranslationSectionsChangedWords { get; set; }
            internal decimal TranslationSectionsChangedCharacters { get; set; }
            internal decimal TranslationSectionsChangedTags { get; set; }




         

            internal List<SDLXLIFF.Comment> Comments { get; set; }

            internal bool SegmentTextUpdated { get; set; }
            internal bool SegmentSegmentStatusUpdated { get; set; }
            internal bool SegmentHasComments { get; set; }



            internal ComparisonSegmentUnit(string segmentId, List<SDLXLIFF.SegmentSection> source, List<SDLXLIFF.SegmentSection> targetOriginal, List<SDLXLIFF.SegmentSection> targetUpdated)
            {
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
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Globalization;

namespace TradosPlugin
{
    public class TagAdjuster
    {
        /// <summary>
        /// Tries the best to adjust the tags from the original source: the tags in the new segments should have the same IDs.
        /// If the number of tags doesn't match the other segment:  
        ///  gets the type, and matches the same types (matching pairs = same anchor or standalone ones), as many as it can, in a linear order.
        /// </summary>
        /// <param name="originalSource"></param>
        /// <param name="hitSource"></param>
        /// <param name="hitTarget"></param>
        public static void AdjustTagsFromOriginalSource(Segment originalSource, ref Segment hitSource, ref Segment hitTarget)
        {
            adjustTags(originalSource, ref hitSource);
            adjustTags(originalSource, ref hitTarget);
        }


        /// <summary>
        /// Adjusts the tags in the 2 segments: changes the tag IDs and anchors in the second segment to match the first one.
        /// Finds the first matching pair for each tag from the first segment. If there's no matching pair, the tag is just skipped.
        /// </summary>
        /// <param name="fromSegment"></param>
        /// <param name="toSegment"></param>
        private static void adjustTags(Segment fromSegment, ref Segment toSegment)
        {
            if (fromSegment == null || toSegment == null) return;
            if (!fromSegment.HasTags || !toSegment.HasTags) return;

            // collect the tags from the segments
            List<Tag> fromTags = new List<Tag>();
            foreach (SegmentElement se in fromSegment.Elements)
            {
                if (se is Tag) fromTags.Add(se as Tag);
            }
            List<Tag> toTags = new List<Tag>();
            foreach (SegmentElement se in toSegment.Elements)
            {
                if (se is Tag) toTags.Add(se as Tag);
            }

            // take each tag from the first segment, and find the first matching pair for it in the second
            // pair: if the tag types match
            foreach (Tag t1 in fromTags)
            {
                // if there are no more tags
                if (toTags.Count == 0) return;
                int i = 0;
                // go to the next tag in the result tags where the type matches // && !DoTagTypesMatch(t1.Type, toTags[i].Type)
                // only use anchor, because the type is not enough, especially if there are unmatched tags #27019
                while (i < toTags.Count && t1.Anchor != toTags[i].Anchor) i++;
                if (i < toTags.Count)
                {
                    // change the properties
                    toTags[i].TagID = t1.TagID;
                    toTags[i].Anchor = t1.Anchor;
                    toTags[i].AlignmentAnchor = t1.AlignmentAnchor;
                    // remove the tag from the second list
                    // as a tag is a reference type, it has been changed in the segment as well
                    toTags.RemoveAt(i);
                }
                // if there's no pair, just move on
            }
        }

        public static bool DoTagTypesMatch(TagType t1, TagType t2)
        {
            if (t1 == t2) return true;
            if (t1 == TagType.End && t2 == TagType.UnmatchedEnd) return true;
            if (t1 == TagType.LockedContent && t2 == TagType.Standalone) return true;
            if (t1 == TagType.LockedContent && t2 == TagType.TextPlaceholder) return true;
            if (t1 == TagType.Standalone && t2 == TagType.LockedContent) return true;
            if (t1 == TagType.Standalone && t2 == TagType.TextPlaceholder) return true;
            if (t1 == TagType.TextPlaceholder && t2 == TagType.LockedContent) return true;
            if (t1 == TagType.TextPlaceholder && t2 == TagType.Standalone) return true;
            if (t1 == TagType.Start && t2 == TagType.UnmatchedStart) return true;
            if (t1 == TagType.UnmatchedEnd && t2 == TagType.End) return true;
            if (t1 == TagType.UnmatchedStart && t2 == TagType.Start) return true;
            return false;
        }
    }
}

using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi.CharacterCountingIterator;

namespace Sdl.Community.AntidoteVerifier.Utils
{
    public static class SegmentExtenstions
    {
        public static string GetString(this ISegment segment, bool includeSegments=false)
        {
            var textVisitor = new TextCollectionVisitor(includeSegments);

            foreach (var item in segment)
            {
                IStructureTag stag = item as IStructureTag;
                if (stag != null) continue;

                item.AcceptVisitor(textVisitor);
            }

            return textVisitor.CollectedText;
        }

        public static IText GetTextAtLocation(this ISegment segment, int startIndex)
        {
            var counter = segment.GetCharacterCountingIterator(startIndex);
            if (counter.CharacterCount < startIndex)
            {
                // the actual count is between this location and the next
                //  - if this is a text node we can point to the exact location inside the text
                IText text = counter.CurrentLocation.ItemAtLocation as IText;
                return text;
            }
            return null;
        }

        public static string Substring(this ISegment segment, int startIndex, int endPosition)
        {
            var counter = segment.GetCharacterCountingIterator(startIndex);
            if (counter.CharacterCount < startIndex)
            {
                // the actual count is between this location and the next
                //  - if this is a text node we can point to the exact location inside the text
                IText text = counter.CurrentLocation.ItemAtLocation as IText;
                if (text != null)
                {
                    var startLocationInsideTextItem = new TextLocation(counter.CurrentLocation, startIndex - counter.CharacterCount);
                    var segmentText = text.Properties.Text;
                    return segmentText.Substring(startLocationInsideTextItem.TextOffset,
                        endPosition - startLocationInsideTextItem.TextOffset);

                }
            }

            return string.Empty;
        }

        public static void Replace(this ISegment segment, int startIndex, int endPosition, string replacementText)
        {
            var counter = segment.GetCharacterCountingIterator(startIndex);
            if (counter.CharacterCount < startIndex)
            {
                // the actual count is between this location and the next
                //  - if this is a text node we can point to the exact location inside the text
                IText text = counter.CurrentLocation.ItemAtLocation as IText;
                if (text != null)
                {
                    var startLocationInsideTextItem = new TextLocation(counter.CurrentLocation, startIndex - counter.CharacterCount);
                    
                    var sb = new StringBuilder(text.ToString());
                    sb.Remove(startLocationInsideTextItem.TextOffset, endPosition - startIndex);
                    sb.Insert(startLocationInsideTextItem.TextOffset, replacementText);
                    text.Properties.Text = sb.ToString();

                }
            }

        }

        private static CharacterCountingIterator GetCharacterCountingIterator(this ISegment segment, int startIndex)
        {
            Location startLocation = new Location(segment, true);


            CharacterCountingIterator counter = new CharacterCountingIterator(startLocation,
                GetStartCountingVisitor,
                GetEndCountingVisitor);
            while (counter.CharacterCount <= startIndex)
            {
                if (!counter.MoveNext())
                {
                    break;
                }
            }
            counter.MovePrevious();
            return counter;
        }

        private static ICharacterCountingVisitor GetStartCountingVisitor()
        {
            return new StartOfItemCharacterCounterNoTagsVisitor();
        }

        private static ICharacterCountingVisitor GetEndCountingVisitor()
        {
            return new EndOfItemCharacterCounterNoTagsVisitor();
        }
    }
}

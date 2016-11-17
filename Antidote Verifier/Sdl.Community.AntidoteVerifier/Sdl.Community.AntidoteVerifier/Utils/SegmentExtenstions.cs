using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi.CharacterCountingIterator;

namespace Sdl.Community.AntidoteVerifier.Utils
{
    public static class SegmentExtenstions
    {
        public static string GetString(this ISegment segment, bool includeSegments=false)
        {
            var textVisitor = new CustomTextCollectionVisitor(segment);

            foreach (var item in segment)
            {
                //IStructureTag stag = item as IStructureTag;
                //if (stag != null) continue;

                item.AcceptVisitor(textVisitor);
            }

            return textVisitor.CollectedText;
        }

        //public static IText GetTextAtLocation(this ISegment segment, int startIndex)
        //{
        //		var counter = segment.GetCharacterCountingIterator(startIndex);
        //		if (counter.CharacterCount < startIndex)
        //		{
        //				// the actual count is between this location and the next
        //				//  - if this is a text node we can point to the exact location inside the text
        //				IText text = counter.CurrentLocation.ItemAtLocation as IText;
        //				return text;
        //		}
        //		return null;
        //}

        public static string Substring(this ISegment segment, int startIndex, int endPosition)
        {
            var segmentText = GetString(segment);
            if (segmentText.Length >= endPosition)
            {
                return segmentText.Substring(startIndex, endPosition - startIndex);
            }

            //var counter = segment.GetCharacterCountingIterator(startIndex);
            //if (counter.CharacterCount < startIndex)
            //{
            //	// the actual count is between this location and the next
            //	//  - if this is a text node we can point to the exact location inside the text
            //	IText text = counter.CurrentLocation.ItemAtLocation as IText;
            //	if (text != null)
            //	{
            //		var startLocationInsideTextItem = new TextLocation(counter.CurrentLocation, startIndex - counter.CharacterCount);
            //		var segmentText = text.Properties.Text;
            //		return segmentText.Substring(startLocationInsideTextItem.TextOffset,
            //				endPosition - startLocationInsideTextItem.TextOffset);

            //	}
            //}

            return string.Empty;
        }

        //displayLanguage = language of the message or the explication 
        public static bool CanReplace(this ISegment segment, int startIndex, int endPosition, string origString, string displayLanguage, ref string message, ref string explication)
        {
            bool ret = false;

            if (segment.Properties.IsLocked)
            {
                ret = false;
                if (displayLanguage == "fr")
                {
                    message = "Antidote ne peut effectuer la correction, car le segment est en lecture seule.";
                    explication = "";
                }
                else
                {
                    message = "Antidote cannot proceed with the correction because the segment is read-only.";
                    explication = "";
                }

            }
            else
            {
                var textVisitor = new CustomTextCollectionVisitor(segment, startIndex, endPosition);
                foreach (var item in segment)
                    item.AcceptVisitor(textVisitor);

                string aString = textVisitor.GetText();

                if (aString == origString)
                {
                    if (textVisitor.RangeContainsTextLocked())
                    {
                        ret = false;
                        if (displayLanguage == "fr")
                        {
                            message = "Antidote ne peut effectuer la correction, car le texte est en lecture seule.";
                            explication = "";
                        }
                        else
                        {
                            message = "Antidote cannot proceed with the correction because the text is read-only.";
                            explication = "";
                        }
                    }
                    else
                    {
                        ret = true;
                    }
                }
                else
                {//other cases
                    ret = false;
                    //default message
                }
            }

            return ret;
        }

        public static void Replace(this ISegment segment, int startIndex, int endPosition, string replacementText)
        {
            var textVisitor = new CustomTextCollectionVisitor(segment, startIndex, endPosition);
            foreach (var item in segment)
                item.AcceptVisitor(textVisitor);

            textVisitor.ReplaceText(replacementText);
        }

        //private static CharacterCountingIterator GetCharacterCountingIterator(this ISegment segment, int startIndex)
        //{
        //		Location startLocation = new Location(segment, true);


        //		CharacterCountingIterator counter = new CharacterCountingIterator(startLocation,
        //				GetStartCountingVisitor,
        //				GetEndCountingVisitor);
        //		while (counter.CharacterCount <= startIndex)
        //		{
        //				if (!counter.MoveNext())
        //				{
        //						break;
        //				}
        //		}
        //		counter.MovePrevious();
        //		return counter;
        //}

        //private static ICharacterCountingVisitor GetStartCountingVisitor()
        //{
        //		return new StartOfItemCharacterCounterNoTagsVisitor();
        //}

        //private static ICharacterCountingVisitor GetEndCountingVisitor()
        //{
        //		return new EndOfItemCharacterCounterNoTagsVisitor();
        //}
    }
}

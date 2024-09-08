using Sdl.Community.AntidoteVerifier.Utils;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;

namespace Sdl.Community.AntidoteVerifier.Extensions
{
    public static class SegmentExtensions
    {
	    public static string GetString(this ISegment segment, bool includeSegments = false) =>
		    segment.ToString();

        //displayLanguage = language of the message or the explication 
        public static bool CanReplace(this ISegment segment, int startIndex, int endPosition, string origString, string displayLanguage, ref string message, ref string explication)
        {
            bool ret;

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

                var aString = textVisitor.GetText();

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
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));

            var currentIndex = 0;

            foreach (var element in segment)
            {
                if (element is IText textElement)
                {
                    var elementLength = textElement.Properties.Text.Length;

                    // Check if the replacement range falls within this text element
                    if (currentIndex <= startIndex && currentIndex + elementLength > endPosition)
                    {
                        // Split the text within this element
                        var beforeText = textElement.Properties.Text.Substring(0, startIndex - currentIndex);
                        var afterText = textElement.Properties.Text.Substring(endPosition - currentIndex);

                        // Set the new text with the replacement
                        textElement.Properties.Text = beforeText + replacementText + afterText;

                        break;
                    }
                    currentIndex += elementLength;
                }
                else
                    currentIndex += element.ToString().Length;
            }
        }
    }
}

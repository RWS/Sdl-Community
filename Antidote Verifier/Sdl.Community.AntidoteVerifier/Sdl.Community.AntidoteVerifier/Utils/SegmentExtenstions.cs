﻿using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.AntidoteVerifier.Utils
{
    public static class SegmentExtenstions
    {
        public static string GetString(this ISegment segment, bool includeSegments=false)
        {
            var textVisitor = new CustomTextCollectionVisitor(segment);

            foreach (var item in segment)
            {
                item.AcceptVisitor(textVisitor);
            }

            return textVisitor.CollectedText;
        }

        public static string Substring(this ISegment segment, int startIndex, int endPosition)
        {
            var segmentText = GetString(segment);
            return segmentText.Length >= endPosition ? segmentText.Substring(startIndex, endPosition - startIndex) : string.Empty;
        }

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
            var textVisitor = new CustomTextCollectionVisitor(segment, startIndex, endPosition);
            foreach (var item in segment)
                item.AcceptVisitor(textVisitor);

            textVisitor.ReplaceText(replacementText);
        }
    }
}

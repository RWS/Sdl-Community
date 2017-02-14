using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdl.Community.Structures.Documents.Records;

namespace Sdl.Community.Report
{
    public class Helper
    {


        public static List<string> GetTextSections(string str)
        {
            var strList = new List<string>();

            var regexDoubleSpaces = new Regex(@"\s{2,}", RegexOptions.Singleline);
            var mcRegexDoubleSpaces = regexDoubleSpaces.Matches(str);


            var previousStart = 0;
            foreach (Match mRegexDoubleSpaces in mcRegexDoubleSpaces)
            {
                if (mRegexDoubleSpaces.Index > previousStart)
                {
                    var startText = str.Substring(previousStart, mRegexDoubleSpaces.Index - previousStart);
                    if (startText.Length > 0)
                        strList.Add(startText);
                }


                var tagText = mRegexDoubleSpaces.Value.Replace(" ", ((char)160).ToString());
                if (tagText.Length > 0)
                    strList.Add(tagText);


                previousStart = mRegexDoubleSpaces.Index + mRegexDoubleSpaces.Length;

            }

            var endText = str.Substring(previousStart);
            if (endText.Length > 0)
                strList.Add(endText);


            return strList;
        }
        public static string GetCompiledSegmentText(List<ContentSection> tcrss, bool includeTags)
        {
            var text = string.Empty;
            foreach (var tcrs in tcrss)
            {
                if (tcrs.RevisionMarker != null && tcrs.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                {
                    //ignore
                }
                else
                {
                    if (tcrs.CntType == ContentSection.ContentType.Text)
                        text += tcrs.Content;
                    else if (includeTags)
                        text += tcrs.Content;


                }
            }

            return text;
        }

    }
}

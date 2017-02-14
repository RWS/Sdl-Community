using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;
using Sdl.Community.Structures.Documents.Records;

namespace Sdl.Community.WPFListView
{
    public class Helper
    {
        private static string ConvertToRtFstr(string str)
        {
            str = str.Replace(@"\", @"\\");
            str = str.Replace(@"{", @"\{");
            str = str.Replace(@"}", @"\}");
            return str;
        }
        private static string ConvertToWordUni(string inputStr)
        {

            inputStr = inputStr.Replace("\n", @"{\line }");
            
            var strOut = string.Empty;
            foreach (var c in inputStr.ToCharArray())
            {
                if (c <= 0x7f)
                {
                    strOut += c;                  
                }
                else
                {              
                    strOut += "\\u" + Convert.ToUInt32(c) + "?";// "\\'83";// "\\ ";

                }
            }
            return strOut;
        }

        public static Span CreateSpanObject(List<ContentSection> sections)
        {
            var span = new Span();
            span.Inlines.Clear();

            foreach (var section in sections)
            {
                if (section.CntType == ContentSection.ContentType.Text)
                {
                    span.Inlines.Add(new Run(section.Content));
                }
                else
                {
                    var run = new Run(section.Content) {Foreground = Brushes.Gray};
                    span.Inlines.Add(run);
                }
            }
            return span;


        }


    }
}

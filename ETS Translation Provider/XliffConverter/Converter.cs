using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace XliffConverter
{
    public static class Converter
    {
        private static XmlSerializer serializer;
        static Converter()
        {
            serializer = new XmlSerializer(typeof(xliff));
        }


        public static xliff ParseXliffString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            // Since XML doesn't like tags, let's temporarily replace the sourceText and targetTranslation, then put
            // them back in after the xml parser has parsed the text.
            const string sourceTextRegex = "<source>(.*?)</source>|<source />";
            const string targetTextRegex = "<target xml:lang=\"[\\w-]+\">(.*?)</target>";
            const string emptySourceTextRegex = "<source></source>";
            const string emptyTargetTextRegex = "<target></target>";
            // Make the regex span multiple lines in case the text includes a newline
            MatchCollection sourceText = Regex.Matches(text, sourceTextRegex, RegexOptions.Singleline);
            text = Regex.Replace(text, sourceTextRegex, emptySourceTextRegex, RegexOptions.Singleline);
            MatchCollection targetText = Regex.Matches(text, targetTextRegex, RegexOptions.Singleline);
            text = Regex.Replace(text, targetTextRegex, emptyTargetTextRegex, RegexOptions.Singleline);
            byte[] byteArray = Encoding.Unicode.GetBytes(text);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                xliff xliff = (xliff)serializer.Deserialize(stream);

                for (int i = 0; i < sourceText.Count; i++)
                {
                    if (sourceText[i].Groups.Count < 2) continue;
                    xliff.File.Body.TranslationUnits[i].SourceText = sourceText[i].Groups[1].Value;
                    xliff.File.Body.TranslationUnits[i].TranslationList.First().Translation.Text = targetText[i].Groups[1].Value;
                    xliff.File.Body.TranslationUnits[i].TranslationList.First().Translation.TargetLanguage = xliff.File.TargetLanguage;
                }
                return xliff;
            }
        }

        public static string PrintXliff(xliff xliff)
        {
            using (StringWriter writer = new StringWriter())
            {
                using (XmlWriter tw = XmlWriter.Create(writer, new XmlWriterSettings() { Indent = true }))
                {
                    serializer.Serialize(tw, xliff);
                }
                return writer.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;lt;", "&lt;");
            }
        }

        internal static string removeXliffTags(string xliffString)
        {
            const string xliffTagRegex = "(<.*?>)";
            xliffString = Regex.Replace(xliffString, xliffTagRegex, "");

            return HttpUtility.HtmlDecode(xliffString);
        }
    }
}

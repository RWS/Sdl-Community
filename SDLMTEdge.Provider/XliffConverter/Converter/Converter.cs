using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Sdl.Community.MTEdge.Provider.XliffConverter.Converter
{
	public static class Converter
	{
		private static readonly XmlSerializer Serializer;

		static Converter()
		{
			Serializer = new XmlSerializer(typeof(Xliff));
		}

		public static Xliff ParseXliffString(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}

			// Since XML doesn't like tags, let's temporarily replace the sourceText and targetTranslation, then put
			// them back in after the xml parser has parsed the text.
			const string sourceTextRegex = "<source>(.*?)</source>|<source />";
			const string targetTextRegex = "<target xml:lang=\"[^\"]*\">(.*?)</target>";
			const string emptySourceTextRegex = "<source></source>";
			const string emptyTargetTextRegex = "<target></target>";

			var sourceText = Regex.Matches(text, sourceTextRegex, RegexOptions.Singleline);
			text = Regex.Replace(text, sourceTextRegex, emptySourceTextRegex, RegexOptions.Singleline);

			var targetText = Regex.Matches(text, targetTextRegex, RegexOptions.Singleline);
			text = Regex.Replace(text, targetTextRegex, emptyTargetTextRegex, RegexOptions.Singleline);

			var byteArray = Encoding.Unicode.GetBytes(text);
            using var stream = new MemoryStream(byteArray);
            var xliff = (Xliff)Serializer.Deserialize(stream);
            for (var i = 0; i < sourceText.Count; i++)
            {
                if (sourceText[i].Groups.Count < 2)
                {
                    continue;
				}

				xliff.File.Body.TranslationUnits[i].SourceText = sourceText[i].Groups[1].Value;
				xliff.File.Body.TranslationUnits[i].TranslationList.First().Translation.TargetLanguage = xliff.File.TargetLanguage;
				try
				{
					xliff.File.Body.TranslationUnits[i].TranslationList.First().Translation.Text = targetText[i].Groups[1].Value;
				}
				catch { }
			}

			return xliff;
        }

		public static string PrintXliff(Xliff xliff)
		{
            using var writer = new StringWriter();
            using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
            {
                Serializer.Serialize(xmlWriter, xliff);
            }

            return writer.ToString()
						 .Replace("&lt;", "<")
						 .Replace("&gt;", ">")
						 .Replace("&amp;lt;", "&lt;");
        }

		internal static string RemoveXliffTags(string xliffString)
		{
			const string xliffTagRegex = "(<.*?>)";
			xliffString = Regex.Replace(xliffString, xliffTagRegex, "");

			return WebUtility.HtmlDecode(xliffString);
		}
	}
}
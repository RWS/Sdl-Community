using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace LanguageWeaverProvider.XliffConverter.Converter
{
	public static class Converter
	{
		const string SourceTextRegex = "<source>(.*?)</source>|<source />";
		const string TargetTextRegex = "<target xml:lang=\"[^\"]*\">(.*?)</target>";
		const string EmptySourceTextRegex = "<source></source>";
		const string EmptyTargetTextRegex = "<target></target>";

		static readonly XmlSerializer Serializer;

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

			var sourceText = Regex.Matches(text, SourceTextRegex, RegexOptions.Singleline);
			text = Regex.Replace(text, SourceTextRegex, EmptySourceTextRegex, RegexOptions.Singleline);

			var targetText = Regex.Matches(text, TargetTextRegex, RegexOptions.Singleline);
			text = Regex.Replace(text, TargetTextRegex, EmptyTargetTextRegex, RegexOptions.Singleline);

			var byteArray = Encoding.Unicode.GetBytes(text);
			using var stream = new MemoryStream(byteArray);
			var xliff = (Xliff)Serializer.Deserialize(stream);

			for (var i = 0; i < sourceText.Count; i++)
			{
				if (sourceText[i].Groups.Count < 2)
				{
					continue;
				}

				xliff.File.Body.TranslationUnits[i].TranslationList.First().Translation.Text = targetText[i].Groups[1].Value;
				xliff.File.Body.TranslationUnits[i].SourceText = sourceText[i].Groups[1].Value;
				xliff.File.Body.TranslationUnits[i].TranslationList.First().Translation.TargetLanguage = xliff.File.TargetLanguage;
			}

			return xliff;
		}

		public static string PrintXliff(Xliff xliff)
		{
			using var writer = new StringWriter();
			using (var tw = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
			{
				Serializer.Serialize(tw, xliff);
			}

			return writer.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;lt;", "&lt;");
		}

		internal static string RemoveXliffTags(string xliffString)
		{
			const string xliffTagRegex = "(<.*?>)";
			xliffString = Regex.Replace(xliffString, xliffTagRegex, "");

			return WebUtility.HtmlDecode(xliffString);
		}
	}
}
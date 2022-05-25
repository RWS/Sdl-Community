﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Sdl.Community.MTCloud.Provider.XliffConverter.Converter
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

			// Make the regex span multiple lines in case the text includes a newline
			var sourceText = Regex.Matches(text, sourceTextRegex, RegexOptions.Singleline);
			text = Regex.Replace(text, sourceTextRegex, emptySourceTextRegex, RegexOptions.Singleline);

			var targetText = Regex.Matches(text, targetTextRegex, RegexOptions.Singleline);
			text = Regex.Replace(text, targetTextRegex, emptyTargetTextRegex, RegexOptions.Singleline);

			var byteArray = Encoding.Unicode.GetBytes(text);
			using (var stream = new MemoryStream(byteArray))
			{
				var xliff = (Xliff)Serializer.Deserialize(stream);

				// represents the Line Feed (new line: \n)
				var softReturn = Convert.ToChar(10).ToString();
				var regExPattern = new Regex("\r\n");

				for (var i = 0; i < sourceText.Count; i++)
				{
					if (sourceText[i].Groups.Count < 2)
					{
						continue;
					}

					// if the target result contains Carriage Return and Line Feed characters(\r\n), replace it with a soft return
					// (otherwise a hard return is added and might crash Studio, the hard return is used for a new paragraph and not to display a new line inside of a segment)
					if (targetText[i].Groups[1].Value.Contains("\r\n"))
					{
						xliff.File.Body.TranslationUnits[i].TranslationList.First().Translation.Text = regExPattern.Replace(targetText[i].Groups[1].Value, softReturn);
					}
					else
					{
						xliff.File.Body.TranslationUnits[i].TranslationList.First().Translation.Text = targetText[i].Groups[1].Value;
					}
					xliff.File.Body.TranslationUnits[i].SourceText = sourceText[i].Groups[1].Value;
					xliff.File.Body.TranslationUnits[i].TranslationList.First().Translation.TargetLanguage = xliff.File.TargetLanguage;
				}
				return xliff;
			}
		}

		public static string PrintXliff(Xliff xliff)
		{
			using (var writer = new StringWriter())
			{
				using (var tw = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
				{
					Serializer.Serialize(tw, xliff);
				}
				return writer.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;lt;", "&lt;");
			}
		}

		internal static string RemoveXliffTags(string xliffString)
		{
			const string xliffTagRegex = "(<.*?>)";
			xliffString = Regex.Replace(xliffString, xliffTagRegex, "");

			return WebUtility.HtmlDecode(xliffString);
		}
	}
}

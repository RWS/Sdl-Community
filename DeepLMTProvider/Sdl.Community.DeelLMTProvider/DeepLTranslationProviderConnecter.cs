using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.DeelLMTProvider.Model;
using Sdl.LanguagePlatform.Core;
using System.Xml;

namespace Sdl.Community.DeepLMTProvider
{
	public class DeepLTranslationProviderConnecter
	{

		public string ApiKey { get; set; }
		private readonly string _pluginVersion = "";
		private readonly string _identifier;

		public DeepLTranslationProviderConnecter(string key, string identifier)
		{
			ApiKey = key;
			_identifier = identifier;

			try
			{
				// fetch the version of the plugin from the manifest deployed
				var pexecutingAsseblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				pexecutingAsseblyPath = Path.Combine(pexecutingAsseblyPath, "pluginpackage.manifest.xml");
				var doc = new XmlDocument();
				doc.Load(pexecutingAsseblyPath);

				if (doc.DocumentElement == null) return;
				foreach (XmlNode n in doc.DocumentElement.ChildNodes)
				{
					if (n.Name == "Version")
					{
						_pluginVersion = n.InnerText;
					}
				}
			}
			catch (Exception e)
			{
				// broad catch here, if anything goes wrong with determining the version we don't want the user to be disturbed in any way
			}
		}

		public string Translate(LanguagePair languageDirection, string sourcetext)
		{
			const string tagOption = @"xml";
			var targetLanguage = languageDirection.TargetCulture.TwoLetterISOLanguageName;
			var sourceLanguage = languageDirection.SourceCulture.TwoLetterISOLanguageName;
			var translatedText = string.Empty;

			try
			{
				var client = new RestClient(@"https://api.deepl.com/v1")
				{
					UserAgent = "SDL Trados 2019 (v" + _pluginVersion + ",id" + _identifier + ")"
				};
				var request = new RestRequest("translate", Method.POST);

				//search for words like this <word> 
				var rgx = new Regex("(\\<\\w+[üäåëöøßşÿÄÅÆĞ]*[^\\d\\W\\\\/\\\\]+\\>)");
				var words = rgx.Matches(sourcetext);

				if (words.Count > 0)
				{
					var matchesIndexes = GetMatchesIndexes(sourcetext, words);
					sourcetext = ReplaceCharacters(matchesIndexes, sourcetext);
				}

				//search for spaces
				var spaceRgx = new Regex("[\\s]+");
				var spaces = spaceRgx.Matches(sourcetext);

				if (spaces.Count > 0)
				{
					var matchesIndexes = GetMatchesIndexes(sourcetext, spaces);
					sourcetext = EncodeSpaces(matchesIndexes, sourcetext);
				}
				//sourcetext = HttpUtility.HtmlEncode(sourcetext);
				//sourcetext = Uri.EscapeDataString(sourcetext);

				request.AddParameter("text", sourcetext);
				request.AddParameter("source_lang", sourceLanguage);
				request.AddParameter("target_lang", targetLanguage);
				//adding this resolve line breaks issue and missing ##login##
				request.AddParameter("preserve_formatting", 1);
				//tag handling cause issues on uppercase words
				request.AddParameter("tag_handling", tagOption);
				//if we add this the formattiong is not right
				//request.AddParameter("split_sentences", 0);
				request.AddParameter("auth_key", ApiKey);

				var response = client.Execute(request).Content;
				var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(response);
				if (translatedObject != null)
				{
					translatedText = translatedObject.Translations[0].Text;
					translatedText = HttpUtility.UrlDecode(translatedText);
					if (words.Count > 0)
					{
						// used to decode < > characters
						translatedText = HttpUtility.HtmlDecode(translatedText);
					}
				}
			}
			catch (WebException e)
			{
				var eReason = Helpers.ProcessWebException(e);
				throw new Exception(eReason);
			}

			return translatedText;
		}

		private string EncodeSpaces(int[] matchesIndexes, string sourceText)
		{
			var spaceRgx = new Regex("([\\s]+){2}");
			var finalText = new StringBuilder();
			var splitedText = sourceText.SplitAt(matchesIndexes).ToList();

			foreach (var text in splitedText)
			{
				var hasMultipleSpace = spaceRgx.IsMatch(text);
				var containsTab = text.Contains('\t');
				if (hasMultipleSpace || containsTab)
				{
					var encodedSpace = Uri.EscapeDataString(text);
					finalText.Append(encodedSpace);
				}
				else
				{
					finalText.Append(text);
				}

			}
			return finalText.ToString();
		}

		private int[] GetMatchesIndexes(string sourcetext, MatchCollection matches)
		{
			var indexes = new List<int>();
			foreach (Match match in matches)
			{
				if (match.Index.Equals(0))
				{
					indexes.Add(match.Length);
				}
				else
				{
					var remainingText = sourcetext.Substring(match.Index + match.Length);
					if (!string.IsNullOrEmpty(remainingText))
					{
						indexes.Add(match.Index);
						indexes.Add(match.Index + match.Length);
					}
					else
					{
						indexes.Add(match.Index);
					}
				}
			}
			return indexes.ToArray();
		}

		private string ReplaceCharacters(int[] indexes, string sourceText)
		{
			var splitedText = sourceText.SplitAt(indexes).ToList();
			var positions = new List<int>();
			for (var i = 0; i < splitedText.Count; i++)
			{
				if (!splitedText[i].Contains("tg"))
				{
					positions.Add(i);
				}
			}

			foreach (var position in positions)
			{
				var originalString = splitedText[position];
				var start = Regex.Replace(originalString, "<", "&lt;");
				var finalString = Regex.Replace(start, ">", "&gt;");
				splitedText[position] = finalString;
			}
			var finalText = string.Empty;
			foreach (var text in splitedText)
			{
				finalText += text;
			}

			return finalText;
		}
	}
}
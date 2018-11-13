using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class BeGlobalConnecter
	{  
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public bool UseClientAuthentication { get; set; }

		public BeGlobalConnecter(string clientId,string clientSecret, bool useClientAuthentication)
		{
			ClientId = clientId;
			ClientSecret = clientSecret;
			UseClientAuthentication = useClientAuthentication;
		}

		public string Translate(LanguagePair languageDirection, string sourcetext)
		{
			//const string tagOption = @"xml";
			var targetLanguage = GetCorespondingLangCode(languageDirection.TargetCulture.ThreeLetterISOLanguageName);
			var sourceLanguage = GetCorespondingLangCode(languageDirection.SourceCulture.ThreeLetterISOLanguageName);
			var translatedText = string.Empty;

			var rgx = new Regex("(\\<\\w+[üäåëöøßşÿÄÅÆĞ]*[^\\d\\W\\\\/\\\\]+\\>)");
			var words = rgx.Matches(sourcetext);

			if (words.Count > 0)
			{
				var matchesIndexes = GetMatchesIndexes(sourcetext, words);
				sourcetext = ReplaceCharacters(matchesIndexes, sourcetext);
			}
			var beGlobalTranslator = new BeGlobalV4Translator("https://translate-api.sdlbeglobal.com", ClientId, ClientSecret,
				sourceLanguage, targetLanguage, "genericnmt", UseClientAuthentication);
			translatedText = beGlobalTranslator.TranslateText(sourcetext);

			return translatedText;
		}

		private string GetCorespondingLangCode(string languageCode)
		{
			if (languageCode.Equals("deu"))
			{ 
				return  "ger";
			}
			return languageCode; 
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

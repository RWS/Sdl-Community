using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.DeelLMTProvider;
using Sdl.Community.DeelLMTProvider.Model;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider
{
	public class DeepLTranslationProviderConnecter{

		public string ApiKey { get; set; }

		public DeepLTranslationProviderConnecter(string key)
		{
			ApiKey = key;
		}

		public string Translate(LanguagePair languageDirection, string sourcetext)
		{
			const string tagOption = @"xml";
			var targetLanguage = languageDirection.TargetCulture.TwoLetterISOLanguageName;
			var sourceLanguage = languageDirection.SourceCulture.TwoLetterISOLanguageName;
			var translatedText = string.Empty;
			try
			{
				var client = new RestClient(@"https://api.deepl.com/v1");
				var request = new RestRequest("translate", Method.POST);

				//search for words like this <word> 
				var rgx = new Regex("(\\<\\w+[üäåëöøßşÿÄÅÆĞ]*[^\\d\\W\\\\/\\\\]+\\>)");
				var words = rgx.Matches(sourcetext);

				if (words.Count>0)
				{
					sourcetext =ReplaceCharacters(sourcetext,words);
				}
				//request.AddParameter("auth_key", ApiKey);
				//request.AddParameter("source_lang", sourceLanguage);
				//request.AddParameter("target_lang", targetLanguage);
				//request.AddParameter("text", sourcetext);

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
					translatedText = HttpUtility.HtmlDecode(translatedText);
				}
			}
			catch (WebException e) 
			{
				var eReason = Helpers.ProcessWebException(e);
				throw new Exception(eReason);
			}

			return translatedText;
		}

		private string ReplaceCharacters(string sourcetext,MatchCollection matches)
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
					//check if there is any text after PI
					var remainingText = sourcetext.Substring(match.Index + match.Length);
					if (!string.IsNullOrEmpty(remainingText))
					{
						//get the position where PI starts to split before
						indexes.Add(match.Index);
						//split after PI
						indexes.Add(match.Index + match.Length);
					}
					else
					{
						indexes.Add(match.Index);
					}
				}
			}
			var splitedText = sourcetext.SplitAt(indexes.ToArray()).ToList();
			var positions = new List<int>();
			for (var i=0;i<splitedText.Count;i++)
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

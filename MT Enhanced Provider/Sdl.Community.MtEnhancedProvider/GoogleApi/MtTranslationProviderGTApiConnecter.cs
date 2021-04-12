/* Copyright 2015 Patrick Porter

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using NLog;
using Sdl.Community.MtEnhancedProvider.Service;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MtEnhancedProvider.GoogleApi
{
	public class MtTranslationProviderGTApiConnecter
	{
		//holds supported languages so we don't have to keep pinging google once the lang has been checked
		//the structure is <targetLang, List<sourceLangs>>
		public static Dictionary<string, List<string>> DictSupportedLangs;

		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly HtmlUtil _htmlUtil;
		
		public MtTranslationProviderGTApiConnecter(string key, HtmlUtil htmlUtil)
		{
			ApiKey = key;
			_htmlUtil = htmlUtil;
		}

		public string ApiKey { get; set; }//for when this is already instantiated but key is changed in dialog

		private void UpdateSupportedLangs(string target)
		{
			var list = GetSourceLangsList(target);
			if (list == null) //returns null if error
			{
				var message = PluginResources.LangPairAuthErrorMsg1 + Environment.NewLine + PluginResources.LangPairAuthErrorMsg2;
				throw new Exception(message); //b/c list will come back null if key is bad
			}

			if (!DictSupportedLangs.ContainsKey(target))
			{
				DictSupportedLangs.Add(target, list);
			}
		}

		public bool IsSupportedLangPair(CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			var sourceLang = GetLanguageCode(sourceCulture);
			var targetLang = GetLanguageCode(targetCulture);
			if (DictSupportedLangs == null)
			{
				DictSupportedLangs = new Dictionary<string, List<string>>();
			}
			
			if (!DictSupportedLangs.ContainsKey(targetLang))
			{
				UpdateSupportedLangs(targetLang);
			}

			return DictSupportedLangs[targetLang].Any(source => source == sourceLang);
		}

		/// <summary>
		/// Used to translate plain text only.
		/// </summary>
		public string Translate(LanguagePair langPair, string text) //this is called for just plain text
		{
			text = HttpUtility.HtmlEncode(text); //we want to HtmlEncode a plain text segment
			var result = DoTranslate(langPair, text, "html");
			return result;
		}

		/// <summary>
		/// Used to translate as html to allow for tag markup
		/// </summary>
		public string Translate(LanguagePair langPair, string text, string format) //this is called for segments with tags
		{
			//here we do not HtmlEncode b/c the tagplacer will do that later..selectively
			var result = DoTranslate(langPair, text, format);
			return result;
		}

		//Credentials are validated only then we try to translate something
		//Translate a dummy text in order to show the error on the UI when user try to close the window.
		public void ValidateCredentials()
		{
			var langPair = new LanguagePair
			{
				SourceCulture = new CultureInfo("en-us"),
				TargetCulture = new CultureInfo("de-de")
			};
			Translate(langPair, string.Empty);
		}

		private string DoTranslate(LanguagePair langPair, string text, string format)
		{
			try
			{
				if (string.IsNullOrEmpty(ApiKey))
				{
					throw new Exception(PluginResources.ApiConnectionGoogleNoKeyErrorMessage);
				}

				var targetLang = langPair.TargetCulture.Name.Equals("fr-HT") ? "ht" : GetLanguageCode(langPair.TargetCulture);
				text = EncodeSpecialChars(text); //all strings should get this final check for characters that seem to break GT api

				//create the url for the translate request
				var url = $"https://translation.googleapis.com/language/translate/v2?key={ApiKey}&q={text}&target={targetLang}";
				if (format != "") //add format if provided
				{
					url += $"&format={format}";
				}

				string result; //this will take the result from the webclient

				using (var webClient = new WebClient())
				{
					webClient.Encoding = Encoding.UTF8;
					try
					{
						result = webClient.DownloadString(url);  //gets us the json data indicating supported source languages for this target
					}
					catch (WebException e) //will come back 400 bad request if there is a problem
					{
						_logger.Error($"{MethodBase.GetCurrentMethod().Name} {e.Message}\n { e.StackTrace}");

						var eReason = GetExceptionReason(e);
						//get our localized error message from the resources file
						throw new Exception(eReason);
					}
				}

				//need to parse results and find key "translatedText" - there should be only one
				var returnedResult = GetTranslation(result);

				var decodedResult = _htmlUtil.HtmlDecode(returnedResult); //google seems to send back html codes at times

				//for some reason, GT is sometimes adding zero-width spaces, aka "bom", aka char(8203)
				//so we need to remove it
				decodedResult = GtRemoveBoms(decodedResult);
				return decodedResult;
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n { ex.StackTrace}");
				throw new Exception(ex.Message);
			}
		}

		private string GtRemoveBoms(string input)
		{
			//this is to deal with google putting in zero-width spaces for some reason, i.e. char(8203)
			//convert returned text to char array
			var chars = input.ToCharArray();
			//remove all char8203 using linq                 
			chars = chars.Where(val => val != (char)8203).ToArray();
			//convert back to a string
			return new string(chars);
		}

		private string GetLanguageCode(CultureInfo ci)
		{
			if (ci.Name == "zh-TW" || ci.Name == "zh-CN") { return ci.Name; } //just get the name for zh-TW which Google can process..google can also process simplified when specifying target as zh-CN but it breaks when you specify that as source??
			if (ci.Name.Equals("nb-NO") || ci.Name.Equals("nn-NO")) return "no";
			if (ci.TwoLetterISOLanguageName.Equals("sr") && ci.DisplayName.ToLower().Contains("latin")) return "sr-Latn";

			var strReturn = ci.TwoLetterISOLanguageName; //if not chinese trad or norweigian get 2 letter code

			//convert tagalog and hebrew for Google
			if (strReturn == "fil") { strReturn = "tl"; }
			if (strReturn == "he") { strReturn = "iw"; }

			return strReturn;
		}

		private string EncodeSpecialChars(string strInput)
		{
			//google can't handle some url encoded characters
			//but if we UrlEncodeUnicode all of them
			//it screws up non-latin alphabet languages
			//...workaround:
			strInput = strInput.Replace("#", "%23");
			strInput = strInput.Replace("&", "%26");
			strInput = strInput.Replace(";", "%3b");

			return strInput;
		}

		public List<string> GetSourceLangsList(string targetLang)
		{
			var list = new List<string>();
			try
			{
				var url = $"https://www.googleapis.com/language/translate/v2/languages?key={ApiKey}&target={targetLang}";
				var result = ""; //this will take the result from the webclient

				using (var webClient = new WebClient{Encoding = Encoding.UTF8})
				{
					try
					{
						result = webClient.DownloadString(url);  //gets us the json data indicating supported source languages for this target
					}
					catch (WebException e) //will come back 400 invalid value if target lang not supported
					{
						_logger.Error($"{MethodBase.GetCurrentMethod().Name} {e.Message}\n { e.StackTrace}");

						var eReason = GetExceptionReason(e);
						return eReason == "Bad Request" ? null : new List<string> {"unsupported"};
						//otherwise, if it is b/c of a non-supported targetLang, eReason will be "Invalid Value"
					}
				}

				list = GetLanguageCodes(result);
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n { ex.StackTrace}");
			}
			return list;
		}

		private string ParseReturnedError(string input)
		{
			//this is different from successful response parses
			var ser = new JavaScriptSerializer();
			var message = string.Empty;

			var dict = ser.Deserialize<Dictionary<string, Dictionary<string, object>>>(input);
			foreach (var strKey in dict.Keys) //this structure gets it out
			{
				object o = dict[strKey];
				var dict2 = (Dictionary<string, object>)o;
				message = dict2["message"].ToString();
			}
			return message;
		}


		private List<string> GetLanguageCodes(string input)
		{
			var jsonObject = JObject.Parse(input)["data"]["languages"].ToArray();
			return jsonObject.Select(language => language["language"].ToString()).ToList();
		}

		private string GetTranslation(string input)
		{
			var jsonObject = JObject.Parse(input)["data"]["translations"];
			return jsonObject[0]["translatedText"].ToString();
		}

		private string GetExceptionReason(WebException exception)
		{
			var myStream = exception.Response.GetResponseStream();
			if (myStream == null) return string.Empty;

			var x = new System.IO.StreamReader(myStream);
			var contents = x.ReadToEnd();
			var reason = ParseReturnedError(contents);

			return reason;
		}
	}
}
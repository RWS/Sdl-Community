using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.TranslatorService;

namespace Sdl.Community.MtEnhancedProvider.MstConnect
{
    internal class ApiConnecter
    {
        private static string _authToken; 
        private static DateTime _tokenExpiresAt; //to keep track of when token expires
        private static List<string> _supportedLangs;
        private MtTranslationOptions _options;
        private string _subscriptionKey=string.Empty;
	    private static readonly string TranslatorUri = @"https://api.cognitive.microsofttranslator.com/";

	   private static readonly Uri ServiceUrl = new Uri("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");
        private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

	    /// <summary>
	    /// This class allows connection to the Microsoft Translation API
	    /// </summary>
	    /// <param name="options"></param>
	    internal ApiConnecter(MtTranslationOptions options)
	    {
		    _options = options;
		    _subscriptionKey = _options.ClientId;
		    if (_authToken == null)
		    {
			    _authToken = GetAuthToken(); //if the class variable has not been set
		    }
		    if (_supportedLangs == null)
		    {
			    _supportedLangs = GetSupportedLanguages(); //if the class variable has not been set
		    }
	    }

	    /// <summary>
        /// Allows static credentials to be updated by the calling program
        /// </summary>
        /// <param name="cid">the client Id obtained from Microsoft</param>
        /// <param name="cst">the client secret obtained from Microsoft</param>
        internal void resetCrd(string cid, string cst)
        {
            _subscriptionKey = cid;
        }
        
        
        /// <summary>
        /// translates the text input
        /// </summary>
        /// <param name="sourceLang"></param>
        /// <param name="targetLang"></param>
        /// <param name="textToTranslate"></param>
        /// <param name="categoryId"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        internal string Translate(string sourceLang, string targetLang, string textToTranslate, string categoryId, string format)
        {
            //convert our language codes
            var sourceLc = convertLangCode(sourceLang);
            var targetLc = convertLangCode(targetLang);

            //check to see if token is null
            if (_authToken == null) _authToken = GetAuthToken();
            
            //check to see if token expired and if so, get a new one
            if (DateTime.Now.CompareTo(_tokenExpiresAt) >= 0) _authToken = GetAuthToken();

            var binding = new BasicHttpBinding();
            var client = new LanguageServiceClient(binding, new EndpointAddress("http://api.microsofttranslator.com/V2/soap.svc"));

            var translatedText = string.Empty;
            if (categoryId != string.Empty)
            {
				//send full language source code in case of custom engine
				translatedText = client.Translate(_authToken, textToTranslate, sourceLang, targetLc, "text/plain",
				categoryId, string.Empty);
			}
            else
            {
                translatedText = client.Translate(_authToken, textToTranslate, sourceLc, targetLc, "text/plain",
               "general", string.Empty);
            } 
          
             return translatedText;
        }

        /// <summary>
        /// Checks of lang pair is supported by MS
        /// </summary>
        /// <param name="sourceLang"></param>
        /// <param name="targetLang"></param>
        /// <returns></returns>
        internal bool isSupportedLangPair(string sourceLang, string targetLang)
        {
            //convert our language codes
            var source = convertLangCode(sourceLang);
            var target = convertLangCode(targetLang);
            
            var sourceSupported = false;
            var targetSupported = false;

            //check to see if both the source and target languages are supported
            foreach (string lang in _supportedLangs)
            {
                if (lang.Equals(source)) sourceSupported = true;
                if (lang.Equals(target)) targetSupported = true;
            }

            if (sourceSupported && targetSupported) return true; //if both are supported return true

            //otherwise return false
            return false;

        }

        private List<string> GetSupportedLanguages()
        {
            //check to see if token is null
            if (_authToken == null) _authToken = GetAuthToken();
            //check to see if token expired and if so, get a new one
            if (DateTime.Now.CompareTo(_tokenExpiresAt) >= 0) _authToken = GetAuthToken();

	        var languageCodeList = new List<string>();
	        try
	        {
		        var client = new RestClient(TranslatorUri);
		        var request = new RestRequest("languages", Method.GET);
		        request.AddHeader("Authorization", _authToken);
		        request.AddParameter("api-version", "3.0");
		        request.AddParameter("scope", "translation");

		        var languageResponse = client.Execute(request).Content;
		        var languages = JsonConvert.DeserializeObject<LanguageResponse>(languageResponse);
		        if (languages != null)
		        {
			        foreach (var language in languages.Translation)
			        {
				        languageCodeList.Add(language.Key);
			        }
		        }
	        }
	        catch (WebException exception)
	        {
				var mesg = ProcessWebException(exception, PluginResources.MsApiFailedGetLanguagesMessage);
		           throw new Exception(mesg);
			}
	        return languageCodeList;
		}

        private string ProcessWebException(WebException e, string message)
        {
            Console.WriteLine("{0}: {1}", message, e);

			// Obtain detailed error information
	        var strResponse = string.Empty;
            using (var response = (HttpWebResponse)e.Response)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(responseStream, System.Text.Encoding.ASCII))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                }
            }
            return string.Format("Http status code={0}, error message={1}", e.Status, strResponse);
        }

        private string GetAuthToken()
        {
            string accessToken = null;
            var task = Task.Run(async () =>
            {
                accessToken = await GetAccessTokenAsync();
            });

            while (!task.IsCompleted)
            {
                System.Threading.Thread.Yield();
            }
            if (task.IsFaulted)
            {
                throw task.Exception;
            }
            if (task.IsCanceled)
            {
                throw new Exception("Timeout obtaining access token.");
            }
            return accessToken;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_subscriptionKey == string.Empty) return string.Empty;

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = ServiceUrl;
                request.Content = new StringContent(string.Empty);
                request.Headers.TryAddWithoutValidation(OcpApimSubscriptionKeyHeader, _subscriptionKey);
                client.Timeout = TimeSpan.FromSeconds(2);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var token = await response.Content.ReadAsStringAsync();
                _tokenExpiresAt = DateTime.Now;
                _authToken = "Bearer " + token;
                return _authToken;
            }
        }

        private string convertLangCode(string languageCode)
        {
            //takes the language code input and converts it to one that MS Translate can use
            if (languageCode.Contains("sr-Cyrl")) return "sr-Cyrl";
            if (languageCode.Contains("sr-Latn")) return "sr-Latn";

	        var ci = new CultureInfo(languageCode); //construct a CultureInfo object with the language code
            
            //deal with chinese..MS Translator has different ones
            if (ci.Name == "zh-TW") return "zh-CHT";
            if (ci.Name == "zh-CN") return "zh-CHS";
            
            // deal with norwegian..MST needs "no" instead of nn or nb
            if (ci.Name.Equals("nb-NO") || ci.Name.Equals("nn-NO")) return "no";
            //otherwise, return the two-letter code
            return ci.TwoLetterISOLanguageName;

        }

        /// <summary>
        /// This method can be used to add translations to the microsoft server.  It is currently not implemented in the plugin
        /// </summary>
        /// <param name="originalText">The original source text.</param>
        /// <param name="translatedText">The updated transated target text.</param>
        /// <param name="sourceLang">The source languge.</param>
        /// <param name="targetLang">The target language.</param>
        /// <param name="user">The MST user to associate the update with (see MS Translator documentation).</param>
        /// <param name="rating">The rating to associate with the update (see MS Translator documentation).</param>
        internal void AddTranslationMethod(string originalText, string translatedText, string sourceLang, string targetLang, string user, string rating)
        {
			//convert our language codes
	        var from = convertLangCode(sourceLang);
	        var to = convertLangCode(targetLang);

            //check to see if token is null
            if (_authToken == null) _authToken = GetAuthToken();
            //check to see if token expired and if so, get a new one
            if (DateTime.Now.CompareTo(_tokenExpiresAt) >= 0) _authToken = GetAuthToken();


            HttpWebRequest httpWebRequest = null;
            WebResponse response = null;

            string addTranslationuri = "http://api.microsofttranslator.com/V2/Http.svc/AddTranslation?originaltext=" + originalText
                                + "&translatedtext=" + translatedText
                                + "&from=" + from
                                + "&to=" + to
                                + "&user=" + user
                                + "&rating=" + rating;

            httpWebRequest = (HttpWebRequest)WebRequest.Create(addTranslationuri);
            httpWebRequest.Headers.Add("Authorization", _authToken);

	        try
	        {
		        response = httpWebRequest.GetResponse();
	        }
	        catch (Exception e)
	        {
		        
	        }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }
    }
}

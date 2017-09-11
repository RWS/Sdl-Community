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
       private static readonly Uri ServiceUrl = new Uri("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");
        private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

        /// <summary>
        /// This class allows connection to the Microsoft Translation API
        /// </summary>
        /// <param name="_options"></param>
        internal ApiConnecter(MtTranslationOptions options)
        {
            this._options = options;
            this._subscriptionKey = this._options.ClientId;
            if (_authToken == null) _authToken = GetAuthToken(); //if the class variable has not been set
            if (_supportedLangs == null) _supportedLangs = getSupportedLangs(); //if the class variable has not been set

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
            string sourceLc = convertLangCode(sourceLang);
            string targetLc = convertLangCode(targetLang);

            //url encode input
            string formattedSourceText = HttpUtility.UrlEncode(textToTranslate);

            //check to see if token is null
            if (_authToken == null) _authToken = GetAuthToken();
            
            //check to see if token expired and if so, get a new one
            if (DateTime.Now.CompareTo(_tokenExpiresAt) >= 0) _authToken = GetAuthToken();

            var binding = new BasicHttpBinding();
            var client = new LanguageServiceClient(binding, new EndpointAddress("http://api.microsofttranslator.com/V2/soap.svc"));

            string translatedText = string.Empty;
            if (categoryId != string.Empty)
            {

				//send full language source code in case of custom engine

				// translatedText = client.Translate(_authToken, textToTranslate, sourceLc, targetLc, "text/plain",
				//categoryId, string.Empty);
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
            string source = convertLangCode(sourceLang);
            string target = convertLangCode(targetLang);
            
            
            bool sourceSupported = false;
            bool targetSupported = false;

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


        private List<string> getSupportedLangs()
        {
            //check to see if token is null
            if (_authToken == null) _authToken = GetAuthToken();

            //check to see if token expired and if so, get a new one
            if (DateTime.Now.CompareTo(_tokenExpiresAt) >= 0) _authToken = GetAuthToken();

            string uri = "http://api.microsofttranslator.com/v2/Http.svc/GetLanguagesForTranslate";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", _authToken); //add token to request headers

            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {

                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(List<string>));

                    List<string> results = (List<string>)dcs.ReadObject(stream);
                    return results;
                }
            }
            catch (WebException e)
            {
                string mesg = ProcessWebException(e, PluginResources.MsApiFailedGetLanguagesMessage);
                throw new Exception(mesg); //throw error up to calling program
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

        private string ProcessWebException(WebException e, string message)
        {
            Console.WriteLine("{0}: {1}", message, e.ToString());

            // Obtain detailed error information
            string strResponse = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)e.Response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(responseStream, System.Text.Encoding.ASCII))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                }
            }
            return String.Format("Http status code={0}, error message={1}", e.Status, strResponse);
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

            CultureInfo ci = new CultureInfo(languageCode); //construct a CultureInfo object with the language code
            
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
            string from = convertLangCode(sourceLang);
            string to = convertLangCode(targetLang);

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
                using (Stream strm = response.GetResponseStream())
                {
                    //Console.WriteLine(String.Format("Translation for {0} has been added successfully.", originaltext));
                }
            }
            catch
            { }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }

        
        private string GenerateTranslateOptionsRequestBody(string category, string contentType, string ReservedFlags, string State, string Uri, string user)
        {
            string body = "<TranslateOptions xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">" +
                "  <Category>{0}</Category>" +
                "  <ContentType>{1}</ContentType>" +
                "  <ReservedFlags>{2}</ReservedFlags>" +
                "  <State>{3}</State>" +
                "  <Uri>{4}</Uri>" +
                "  <User>{5}</User>" +
                "</TranslateOptions>";
            return string.Format(body, category, contentType, ReservedFlags, State, Uri, user);
        }

        

    }
}

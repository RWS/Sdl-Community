using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;

namespace Sdl.Community.MtEnhancedProvider.MstConnect
{
    internal class ApiConnecter
    {
        private static string authToken; 
        private static DateTime tokenExpiresAt; //to keep track of when token expires
        private static List<string> supportedLangs;
        private MtTranslationOptions options;
        private string cid; 
        private string cst;

        /// <summary>
        /// This class allows connection to the Microsoft Translation API
        /// </summary>
        /// <param name="_options"></param>
        internal ApiConnecter(MtTranslationOptions _options)
        {
            this.options = _options;
            this.cid = options.ClientID;
            this.cst = options.ClientSecret;
            if (authToken == null) authToken = getAuthToken(); //if the class variable has not been set
            if (supportedLangs == null) supportedLangs = getSupportedLangs(); //if the class variable has not been set

        }

               
        
        /// <summary>
        /// Allows static credentials to be updated by the calling program
        /// </summary>
        /// <param name="cid">the client Id obtained from Microsoft</param>
        /// <param name="cst">the client secret obtained from Microsoft</param>
        internal void resetCrd(string cid, string cst)
        {
            this.cst = cst;
            this.cid = cid;
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
            if (authToken == null) authToken = getAuthToken();

            //check to see if token expired and if so, get a new one
            if (DateTime.Now.CompareTo(tokenExpiresAt) >= 0) authToken = getAuthToken();


            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + formattedSourceText + "&from=" + sourceLc + "&to=" + targetLc + "&contentType=" + format;

            //add category ID if applicable
            if (!categoryId.Equals(""))
                uri += "&category=" + categoryId;


            //delete the follwoing line for production...only to be able to trace http calls using Fiddler
            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            httpWebRequest.Headers.Add("Authorization", authToken);


            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    string translation = (string)dcs.ReadObject(stream);
                    return translation; //return
                }
            }
            catch (WebException e)
            {
                string errorResponse = ProcessWebException(e, PluginResources.MsApiFailedToTranslateMessage);
                //in case our expiration check didn't work
                bool returnedExpiredToken = errorResponse.Contains("Message: The incoming token has expired.");
                bool returnedBadCatID = errorResponse.Contains("Message: Invalid category");

                if (returnedExpiredToken)
                {//if the reason is that the token is expired
                    authToken = getAuthToken();
                    string x = Translate(sourceLang, targetLang, textToTranslate, categoryId, format);
                }
                else if (returnedBadCatID)
                    throw new Exception(PluginResources.MsApiCategoryIdErrorMessage);
                else throw new Exception(errorResponse); //to throw other errors up to calling program

            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }

            return "";
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
            foreach (string lang in supportedLangs)
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
            if (authToken == null) authToken = getAuthToken();

            //check to see if token expired and if so, get a new one
            if (DateTime.Now.CompareTo(tokenExpiresAt) >= 0) authToken = getAuthToken();

            string uri = "http://api.microsofttranslator.com/v2/Http.svc/GetLanguagesForTranslate";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken); //add token to request headers

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

        private string getAuthToken()
        {
            AdmAccessToken admToken;
            string headerValue;
            //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            //Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
            try
            {
                AdmAuthentication admAuth = new AdmAuthentication(cid, cst);
                admToken = admAuth.GetAccessToken();
                TimeSpan span = new TimeSpan(0, 0, int.Parse(admToken.expires_in)); //set a timespan for the time that the token expires
                tokenExpiresAt = DateTime.Now.Add(span);
                headerValue = "Bearer " + admToken.access_token;
            }
            catch (WebException)
            {
                throw new Exception(PluginResources.MsApiBadCredentialsMessage);// + prompt);
            }

            return headerValue;
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
            if (authToken == null) authToken = getAuthToken();
            //check to see if token expired and if so, get a new one
            if (DateTime.Now.CompareTo(tokenExpiresAt) >= 0) authToken = getAuthToken();


            HttpWebRequest httpWebRequest = null;
            WebResponse response = null;

            string addTranslationuri = "http://api.microsofttranslator.com/V2/Http.svc/AddTranslation?originaltext=" + originalText
                                + "&translatedtext=" + translatedText
                                + "&from=" + from
                                + "&to=" + to
                                + "&user=" + user
                                + "&rating=" + rating;

            httpWebRequest = (HttpWebRequest)WebRequest.Create(addTranslationuri);
            httpWebRequest.Headers.Add("Authorization", authToken);

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

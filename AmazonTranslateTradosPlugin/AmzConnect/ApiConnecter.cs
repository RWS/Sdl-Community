using System.Collections.Generic;
using System.Globalization;
using Amazon.Runtime.CredentialManagement;
using Amazon.Translate;
using Amazon.Translate.Model;

namespace Sdl.Community.AmazonTranslateTradosPlugin.AmzConnect
{
    internal class ApiConnecter
    {
        private static List<string> _supportedLangs;
        private MtTranslationOptions _options;
        private string _accessKey = string.Empty;
        private string _secretKey = string.Empty;

        /// <summary>
        /// This class allows connection to the AmazonTranslate API
        /// </summary>
        /// <param name="options"></param>
        internal ApiConnecter(MtTranslationOptions options)
        {
            _options = options;
            _accessKey = _options.AccessKey;
            _secretKey = options.SecretKey;


            if (_supportedLangs == null)
            {
                _supportedLangs = GetSupportedLanguages(); //if the class variable has not been set
            }
        }

        /// <summary>
        /// Allows static credentials to be updated by the calling program
        /// </summary>
        /// <param name="ak">the client Id obtained from Microsoft</param>
        /// <param name="sk">the client secret obtained from Microsoft</param>
        internal void resetCrd(string ak, string sk)
        {
            _accessKey = ak;
            _secretKey = sk;
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
        internal string Translate(string sourceLang, string targetLang, string textToTranslate)
        {
            //convert our language codes
            var sourceLc = convertLangCode(sourceLang);
            var targetLc = convertLangCode(targetLang);


            var translatedText = string.Empty;


            Amazon.Runtime.AWSCredentials awsCreds = null;
            Amazon.RegionEndpoint awsRegion = null;
            

            if (_options.SelectedAuthType == MtTranslationOptions.AWSAuthType.Profile)
            {
                var credentialsFile = new SharedCredentialsFile(); //TODO: always SharedCredentialsFile?
                var prof = credentialsFile.TryGetProfile(_options.ProfileName, out CredentialProfile profile); //TODO: add in error-handling
                awsCreds = profile.GetAWSCredentials(credentialsFile);
                awsRegion = profile.Region;
            }
            else
            {
                awsCreds = new Amazon.Runtime.BasicAWSCredentials(_accessKey, _secretKey);
                awsRegion = Amazon.RegionEndpoint.GetBySystemName(_options.RegionName);
            }

            var tclient = new AmazonTranslateClient(awsCreds, awsRegion);
            var trequest = new TranslateTextRequest();
            trequest.SourceLanguageCode = sourceLc;
            trequest.TargetLanguageCode = targetLc;
            trequest.Text = textToTranslate;
            try
            {
                var result = tclient.TranslateText(trequest);
                System.Diagnostics.Debug.WriteLine(result.TranslatedText);
                translatedText = result.TranslatedText;
            }
            catch (Amazon.Translate.AmazonTranslateException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw e;
            }
            return translatedText;
        }


        /// <summary>
        /// Checks if lang pair is supported by AWS
        /// </summary>
        /// <param name="sourceLang"></param>
        /// <param name="targetLang"></param>
        /// <returns></returns>
        internal bool IsSupportedLangPair(string sourceLang, string targetLang)
        {	
            return true;  
        }

        private List<string> GetSupportedLanguages()
        {
            List<string> languageCodeList = new List<string> {
                "en",
                "es",
                "ar",
                "de",
                "fr",
                "pt",
                "zh",
                "zh-TW",
                "cs",
                "ja",
                "ru",
                "tr"
            };
            return languageCodeList;
        }


        private string convertLangCode(string languageCode)
        {
            //takes the language code input and converts it to one that AWS Translate can use
            //			if (languageCode.Contains("sr-Cyrl")) return "sr-Cyrl";
            //			if (languageCode.Contains("sr-Latn")) return "sr-Latn";

            var ci = new CultureInfo(languageCode); //construct a CultureInfo object with the language code

            //deal with chinese..MS Translator has different ones
            if (ci.Name == "zh-TW") return "zh-TW";
            //			if (ci.Name == "zh-CN") return "zh-CHS";

            // deal with norwegian..MST needs "no" instead of nn or nb
            if (ci.Name.Equals("nb-NO") || ci.Name.Equals("nn-NO")) return "no";
            //otherwise, return the two-letter code
            return ci.TwoLetterISOLanguageName;

        }

    }
}

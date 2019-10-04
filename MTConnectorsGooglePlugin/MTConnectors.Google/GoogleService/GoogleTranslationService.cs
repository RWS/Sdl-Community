using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using NLog;

namespace Sdl.LanguagePlatform.MTConnectors.Google.GoogleService
{
    internal class GoogleTranslationService
    {
        private static Dictionary<string, string> _codeMappings =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {{"ga", "ga-IE"}, {"cy", "cy-GB"}, {"tl", "fil-PH"}, {"zh", "zh-CHS"}};

        private static Dictionary<string, System.Globalization.CultureInfo> _languageList;
        private static List<LanguagePair> _supportedLanguageDirections;
        private static object _languageQueryLock = new object();

        private System.Web.Script.Serialization.JavaScriptSerializer _serializer;
        private readonly QueryRequestBuilder _queryRequestBuilder;
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        public GoogleTranslationService(LanguagePair languageDirection, QueryRequestBuilder queryRequestBuilder)
        {
            _queryRequestBuilder = queryRequestBuilder ?? throw new ArgumentNullException();

            if (languageDirection == null)
            {
                throw new ArgumentNullException();
            }

            if (!IsSupported(languageDirection.SourceCulture))
            {
                throw new ArgumentException(string.Format(
                                        PluginResources.EMSG_SourceCultureUnsupported,
                                        languageDirection.SourceCultureName));
            }

            if (!IsSupported(languageDirection.TargetCulture))
            {
                throw new ArgumentException(string.Format(
                                        PluginResources.EMSG_TargetCultureUnsupported,
                                        languageDirection.TargetCultureName));
            }

            LanguageDirection = languageDirection;
            SourceLanguageId = GetLanguageId(languageDirection.SourceCulture);
            TargetLanguageId = GetLanguageId(languageDirection.TargetCulture);
            _serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        }

        public GoogleTranslationService(QueryRequestBuilder queryRequestBuilder)
        {
            _queryRequestBuilder = queryRequestBuilder ?? throw new ArgumentNullException();
            GetGoogleTranslateSupportedLanaguagesWithLock();
        }

        public LanguagePair LanguageDirection { get; }

        public string SourceLanguageId { get; }

        public string TargetLanguageId { get; }


        #region {retrieval of Google supported languages}
        private static string Response2String(HttpWebResponse response)
        {
            if (response == null)
            {
                return null;
            }

            using (response)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }


        /// <summary>
        /// Tries a language query to but closes the connection after 
        /// reading the status header an OK status validates that the key is correct
        /// </summary>
        /// <returns></returns>
        public bool IsConnectionAndAPIKeyValid()
        {
            try
            {
                var request = _queryRequestBuilder.BuildLanguageQueryRequest();
                using (var response = request.Send())
                {
                    bool status = response.StatusCode == HttpStatusCode.OK;
                    return status;
                }
            }
            catch
            {
                return false;
            }
        }

        private void GetGoogleTranslateSupportedLanguages()
        {
            var request = _queryRequestBuilder.BuildLanguageQueryRequest();
            using (var response = request.Send())
            {
                var supportedLangString = Response2String(response);
                if (string.IsNullOrEmpty(supportedLangString))
                {
                    return;
                }

                string[] googleSupportedLanguages =
                (from Match m in Regex.Matches(supportedLangString, @"""language"":[ ]*""(?<lang>[a-zA-Z\-]*)""")
                 select m.Groups["lang"].Value).ToArray<string>();

                LoadDotNetLanguageList(googleSupportedLanguages);

                _supportedLanguageDirections = new List<Core.LanguagePair>();

                // NOTE it seems that G-Translate supports translation from any into any language. If this
                // is not correct, the following code below must be changed.
                LoadSupportLanguageDirections();
            }
        }

        private void LoadSupportLanguageDirections()
        {
            foreach (KeyValuePair<string, System.Globalization.CultureInfo> kvp1 in _languageList)
            {
                foreach (KeyValuePair<string, System.Globalization.CultureInfo> kvp2 in _languageList)
                {
                    if (kvp1.Key != kvp2.Key)
                    {
                        _supportedLanguageDirections.Add(new Core.LanguagePair(kvp1.Value, kvp2.Value));
                    }
                }
            }
        }

        private void LoadDotNetLanguageList(string[] googleSupportedLangs)
        {
            _languageList = new Dictionary<string, System.Globalization.CultureInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (string lng in googleSupportedLangs)
            {
                // map languages as required by .Net:
                if (!_codeMappings.TryGetValue(lng, out var dotnetCode))
                {
                    dotnetCode = lng;
                }

                var culture = CultureInfoExtensions.GetCultureInfo(dotnetCode, true);
                if (culture != null && !_languageList.ContainsKey(lng))
                {
                    _languageList.Add(lng, culture);
                }
                else
                {
                    _logger.Warn($"Could not resolve Google language {lng}");
                }
            }
        }

        private void GetGoogleTranslateSupportedLanaguagesWithLock()
        {
            lock (_languageQueryLock)
            {
                if (_languageList == null)
                {
                    try
                    {
                        GetGoogleTranslateSupportedLanguages();
                    }
                    catch (Exception ex)
                    {
                        // hide the exception, most probably the authentication is done yet.
                        _logger.Error(ex);
                    }
                }
            }
        }
        #endregion

        public List<LanguagePair> GetSupportedLanguageDirections()
        {
            GetGoogleTranslateSupportedLanaguagesWithLock();
            return _supportedLanguageDirections;
        }

        public bool IsSupported(System.Globalization.CultureInfo culture)
        {
            GetGoogleTranslateSupportedLanaguagesWithLock();
            if (culture == null)
            {
                return false;
            }

            return GetLanguageId(culture) != null;
        }

        private string GetLanguageId(System.Globalization.CultureInfo culture)
        {
            GetGoogleTranslateSupportedLanaguagesWithLock();
            foreach (KeyValuePair<string, System.Globalization.CultureInfo> kvp in _languageList)
            {
                if (AreCompatible(culture, kvp.Value))
                {
                    return kvp.Key;
                }
            }

            return null;
        }

        private bool AreCompatible(System.Globalization.CultureInfo sdlLanguage, System.Globalization.CultureInfo googleLanguage)
        {
            if (sdlLanguage == null || googleLanguage == null)
            {
                throw new ArgumentNullException();
            }

            if (googleLanguage.Name == "no" && (sdlLanguage.Name == "nb-NO" || sdlLanguage.Name == "nn-NO"))
            {
                return true;
            }

            return CultureInfoExtensions.AreCompatible(sdlLanguage, googleLanguage);
        }

        public bool AreCompatible(LanguagePair sdlLanguagePair, LanguagePair googleLanguagePair)
        {
            if (sdlLanguagePair == null || googleLanguagePair == null)
            {
                throw new ArgumentNullException();
            }

            if (sdlLanguagePair.SourceCulture == null
                || sdlLanguagePair.TargetCulture == null
                || googleLanguagePair.SourceCulture == null
                || googleLanguagePair.TargetCulture == null)
            {
                return false;
            }

            return AreCompatible(sdlLanguagePair.SourceCulture, googleLanguagePair.SourceCulture)
                && AreCompatible(sdlLanguagePair.TargetCulture, googleLanguagePair.TargetCulture);
        }

        private TranslationResult<TranslationResponse> ProcessRequest(Request request)
        {
            try
            {
                var httpWebResponse = request.Send();
                if (httpWebResponse == null)
                {
                    return null;
                }

                var enc = ResolveResponseEncoding(httpWebResponse);

                string result = null;
                using (StreamReader rdr = new StreamReader(httpWebResponse.GetResponseStream(), enc, true))
                {
                    result = rdr.ReadToEnd();
                    rdr.Close();
                }

                httpWebResponse.Close();

                var standardResponse = _serializer.Deserialize<StandardResponse<TranslationResult<TranslationResponse>>>(result);

                if (standardResponse?.data == null)
                {
                    throw new Exception(PluginResources.EMSG_EmptyResponse);
                }

                if (standardResponse.responseStatus != HttpStatusCode.OK)
                {
                    throw new Exception(string.Format(
                                            PluginResources.EMSG_ErrorResponse,
                                            standardResponse.responseDetails.ToString()));
                }

                TranslationResult<TranslationResponse> responses = null;
                if (standardResponse.data != null)
                {
                    responses = standardResponse.data;
                    responses.responseDetails = standardResponse.responseDetails;
                    responses.responseStatus = standardResponse.responseStatus;
                }

                return responses;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        private Encoding ResolveResponseEncoding(HttpWebResponse httpWebResponse)
        {
            Encoding enc = null;
            if (!string.IsNullOrEmpty(httpWebResponse.ContentEncoding))
            {
                enc = Encoding.GetEncoding(httpWebResponse.ContentEncoding);
            }

            if (enc == null && !string.IsNullOrEmpty(httpWebResponse.ContentType))
            {
                // get the charset from the content type value by building up key/value pairs of the content type
                Dictionary<string, string> contentTypes = new Dictionary<string, string>();

                // slightly more generic than necessary for future extensions
                foreach (string keyValuePair in httpWebResponse.ContentType.Split(';'))
                {
                    string[] kvp = keyValuePair.Split('=');
                    if (kvp.Length == 2)
                    {
                        contentTypes.Add(kvp[0].Trim(), kvp[1].Trim());
                    }
                    else if (kvp.Length == 1)
                    {
                        contentTypes["contentType"] = kvp[0].Trim();
                    }
                }

                string charsetSpecifier = contentTypes["charset"];

                if (charsetSpecifier != null)
                {
                    enc = Encoding.GetEncoding(charsetSpecifier);
                }
            }

            if (enc == null)
            {
                // fallback to Windows 1252 if we didn't find any helpful information
                enc = Encoding.GetEncoding("windows-1252");
            }

            return enc;
        }


        public Segment Translate(Segment seg)
        {
            var request = _queryRequestBuilder.BuildTranslateRequest(SourceLanguageId, TargetLanguageId);
            var segmentConverter = new SegmentConverter(seg);
            request.AddField("q", segmentConverter.ConvertSourceSegmentToText());

            var translation = ProcessRequest(request);

            if (translation?.translations != null && translation.translations.Count == 1 && translation.translations[0] != null)
            {
                // Trap Specific Failure
                if (translation.responseStatus == HttpStatusCode.Forbidden)
                {
                    throw new ApplicationException(string.Format(PluginResources.EMSG_GoogleForbidden, (int)translation.responseStatus, translation.responseStatus.ToString(), translation.responseDetails));
                }

                if (translation.responseStatus != HttpStatusCode.OK)
                {
                    throw new ApplicationException(string.Format(PluginResources.EMSG_GoogleGeneralFailure, (int)translation.responseStatus, translation.responseStatus.ToString(), translation.responseDetails));
                }

                if (translation.responseData?.translatedText != null)
                {
                    var resultSegment = segmentConverter.ConvertTargetTextToSegment(translation.responseData.translatedText).Duplicate();
                    return resultSegment;
                }

                return null;
            }

            return null;
        }
    }
}

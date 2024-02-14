using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMProvider;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace TradosPlugin
{
    public class SettingsURICreator
    {
        #region Parameter names

        private const string providersName = "Providers";
        private const string canReverseName = "CanReverseLangDir";
        private const string strictSublangName = "StrictSublang";
        private const string separatorTagName = "provider";
        private const string concCaseSens = "ConcCaseSens";
        private const string concNumEquiv = "ConcNumEquiv";

        //private string getTypeName(MemoQTMProviderTypes type)
        //{
        //    if (type == MemoQTMProviderTypes.LanguageTerminal) return "memoQLT";
        //    if (type == MemoQTMProviderTypes.LTResourceServer) return "memoQLTResource";
        //    if (type == MemoQTMProviderTypes.MemoQServer) return "memoQServer";
        //    if (type == MemoQTMProviderTypes.MemoQServer) return "unknown";
        //    throw new InvalidOperationException("This provider type has no name yet.");
        //}
        #endregion

        // !!! if you need other parameters to identify a provider, please, use the URI builder
        // otherwise it will be missing from the URI, and Trados will not send it back to the factory

        public static Uri CreateURIFromSettings(GeneralTMSettings generalSettings, List<MemoQTMSettings> providerSettings)
        {
            var dummyURL = "https://www.memoq.com/";
            /// <summary>
            /// Has properties and methods to store the parameters for the provider, and can have as many key-value pairs as needed (string, string). 
            /// They can be accessed through a string indexer.
            /// </summary>
            TranslationProviderUriBuilder uriBuilder;
            uriBuilder = new TranslationProviderUriBuilder(new Uri(dummyURL));
            //uriBuilder.Password = this.password = password;
            //uriBuilder.UserName = this.userName = username;

            uriBuilder[canReverseName] = generalSettings.CanReverseLangDir.ToString();
            uriBuilder[strictSublangName] = generalSettings.StrictSublanguageMatching.ToString();
            uriBuilder[concCaseSens] = generalSettings.ConcordanceCaseSensitive.ToString();
            uriBuilder[concNumEquiv] = generalSettings.ConcordancNumericEquivalence.ToString();


            // the provider settings
            // each of them will be one xml string, between <provider> tags
            var sb = new StringBuilder();
            if (providerSettings == null || providerSettings.Count == 0) uriBuilder[providersName] = "";
            else
            {
                foreach (var settings in providerSettings)
                {
                    sb.Append("<" + separatorTagName + ">");
                    sb.Append(settings.ToXmlString());
                    sb.Append("</" + separatorTagName + ">");
                }
            }
            uriBuilder[providersName] = sb.ToString();
            return uriBuilder.Uri;

        }

        public static GeneralTMSettings CreateSettingsFromURI(Uri uriWithParameters, out List<MemoQTMSettings> providerSettings)
        {
            var uriBuilder = new TranslationProviderUriBuilder(uriWithParameters);
            // load properties
            bool rev = true, strict = false, cCase = false, cNum = false;
            if (uriBuilder[canReverseName] != null)
            {
                rev = bool.Parse(uriBuilder[canReverseName]);
            }
            if (uriBuilder[strictSublangName] != null)
            {
                strict = bool.Parse(uriBuilder[strictSublangName]);
            }

            if (uriBuilder[concCaseSens] != null)
            {
                cCase = bool.Parse(uriBuilder[concCaseSens]);
            }
            if (uriBuilder[concNumEquiv] != null)
            {
                cNum = bool.Parse(uriBuilder[concNumEquiv]);
            }

            var genSet = new GeneralTMSettings(rev, strict, cCase, cNum);
            providerSettings = new List<MemoQTMSettings>();
            if (uriBuilder[providersName] != null)
            {
                // provider-specific settings
                // xml strings divided by a separator tag
                var nextIx = 0;
                while ((nextIx = uriBuilder[providersName].IndexOf("<" + separatorTagName + ">", nextIx)) > -1)
                {
                    nextIx += separatorTagName.Length + 2;
                    var endIx = uriBuilder[providersName].IndexOf("</" + separatorTagName + ">", nextIx);
                    var settingsXML = uriBuilder[providersName].Substring(nextIx, endIx - nextIx);
                    providerSettings.Add(MemoQTMSettings.CreateFromXML(settingsXML));
                }
            }
            return genSet;
        }

    }
}

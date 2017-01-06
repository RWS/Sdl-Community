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
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MtEnhancedProvider
{
    /// <summary>
    /// This class is used to hold the provider plug-in settings. 
    /// All settings are automatically stored in a URI.
    /// </summary>
    public class MtTranslationOptions
    {

        private static string _apiKey;
        private static string _clientsecret;
        private static string _clientid;
        const string msTranslatorString = "Microsoft Translator"; //these strings should not be localized or changed and are therefore hard-coded as constants
        const string gTranslateString = "Google Translate"; //these strings should not be localized or changed and are therefore hard-coded as constants
        
        #region "TranslationMethod"
        //The translation method affects when/if the plugin gets called by Studio
        public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;
        #endregion




        #region "TranslationProviderUriBuilder"
        TranslationProviderUriBuilder _uriBuilder;

        public MtTranslationOptions()
        {
            _uriBuilder = new TranslationProviderUriBuilder(MtTranslationProvider.ListTranslationProviderScheme);
        }

        public MtTranslationOptions(Uri uri)
        {
            _uriBuilder = new TranslationProviderUriBuilder(uri);
        }
        #endregion

        

        private string sendPlainTextOnly
        {
            get { return GetStringParameter("sendplaintextonly"); }
            set { SetStringParameter("sendplaintextonly", value); }
        }

        private string UseCatId
        {
            get { return GetStringParameter("usecatid"); }
            set { SetStringParameter("usecatid", value); }
        }

        public string resendDrafts
        {
            get { return GetStringParameter("resenddrafts"); }
            set { SetStringParameter("resenddrafts", value); }
        }
        

        #region "EditFiles"

        public bool UsePreEdit
        {
            get { return Convert.ToBoolean(usePreEdit); }
            set { usePreEdit = value.ToString(); }
        }

        public bool UsePostEdit
        {
            get { return Convert.ToBoolean(usePostEdit); }
            set { usePostEdit = value.ToString(); }
        }

        private string usePreEdit
        {
            get { return GetStringParameter("usepreedit"); }
            set { SetStringParameter("usepreedit", value); }
        }

        private string usePostEdit
        {
            get { return GetStringParameter("usepostedit"); }
            set { SetStringParameter("usepostedit", value); }
        }

        public string PreLookupFilename
        {
            get { return GetStringParameter("prelookupfilename"); }
            set { SetStringParameter("prelookupfilename", value); }
        }

        public string PostLookupFilename
        {
            get { return GetStringParameter("postlookupfilename"); }
            set { SetStringParameter("postlookupfilename", value); }
        }

        public enum ProviderType
        {
            GoogleTranslate = 1,
            MicrosoftTranslator = 2,
            None = 0
        }

        public static string GetProviderTypeDescription(ProviderType type)
        {
            if (type == ProviderType.GoogleTranslate)
                return gTranslateString; //these strings should not be localized and are therefore hard-coded
            else if (type == ProviderType.MicrosoftTranslator)
                return msTranslatorString; //these strings should not be localized and are therefore hard-coded
            else
                return "";
        }

        public static ProviderType GetProviderType(string typeString)
        {
            //we changed the options provider type to use resource strings..but if a user migrates a project to a machine with a different culture then it will be a problem
            //the solution seems to be to not translate the names for 'Google Translate' and 'Microsoft Translator' ...they both leave it untranslated in their documentation in other languages
            if (typeString == null)
                return ProviderType.None;
            else if (typeString.Equals(gTranslateString)) //these strings should not be localized and are therefore hard-coded
                return ProviderType.GoogleTranslate;
            else if (typeString.Equals(msTranslatorString)) //these strings should not be localized and are therefore hard-coded
                return ProviderType.MicrosoftTranslator;
            else
                return ProviderType.None;
        }
        
        public ProviderType SelectedProvider
        {
            get 
            {
                return GetProviderType(GetStringParameter("selectedprovider"));
            }
            set 
            {
                string typestring = GetProviderTypeDescription(value);
                SetStringParameter("selectedprovider", typestring); 
            }
        }

        #endregion





        public string ApiKey
        {
            get { return _apiKey; } //the apiKey is going to be held in a static variable so we don't have to get it from credential store all the time
            set { _apiKey = value; }
        }

        public string ClientId
        {
            get { return _clientid; } //the creds are going to be held in a static variable so we don't have to get it from credential store all the time
            set { _clientid = value; }
        }

        public string ClientSecret
        {
            get { return _clientsecret; } //the creds are going to be held in a static variable so we don't have to get it from credential store all the time
            set { _clientsecret = value; }
        }

        public bool PersistGoogleKey
        {
            get;
            set;
        }

        public bool PersistMicrosoftCreds
        {
            get;
            set;
        }

        public bool ResendDrafts //we'll access this from other classes..converting to and from string for purposes of our uri setter/getter above
        {
            get { return Convert.ToBoolean(resendDrafts); }
            set { resendDrafts = value.ToString(); }
        }

        public bool SendPlainTextOnly //we'll access this from other classes..converting to and from string for purposes of our uri setter/getter above
        {
            get { return Convert.ToBoolean(sendPlainTextOnly); }
            set { sendPlainTextOnly = value.ToString(); }
        }

        public bool UseCatID //we'll access this from other classes..converting to and from string for purposes of our uri setter/getter above
        {
            get { return Convert.ToBoolean(UseCatId); }
            set { UseCatId = value.ToString(); }
        }

        public string CatId
        {
            get { return GetStringParameter("catid"); }
            set { SetStringParameter("catid", value); }
        }


        #region "SetStringParameter"
        private void SetStringParameter(string p, string value)
        {
            _uriBuilder[p] = value;
        }
        #endregion

        #region "GetStringParameter"
        private string GetStringParameter(string p)
        {
            string paramString = _uriBuilder[p];
            return paramString;
        }
        #endregion


        #region "Uri"
        public Uri Uri
        {
            get
            {
                return _uriBuilder.Uri;
            }
        }
        #endregion
    }

    
}

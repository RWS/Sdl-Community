﻿/* 

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
using Newtonsoft.Json;
using Sdl.Community.AmazonTranslateProvider;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.AmazonTranslateTradosPlugin
{
    /// <summary>
    /// This class is used to hold the provider plug-in settings. 
    /// All settings are automatically stored in a URI.
    /// </summary>
    public class MtTranslationOptions
    {


        private static string _secretkey;
        private static string _accesskey;

        //The translation method affects when/if the plugin gets called by Studio
        public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;

        TranslationProviderUriBuilder _uriBuilder;

        public MtTranslationOptions()
        {
            _uriBuilder = new TranslationProviderUriBuilder(MtTranslationProvider.TranslationProviderScheme);
        }

        public MtTranslationOptions(Uri uri)
        {
            _uriBuilder = new TranslationProviderUriBuilder(uri);
        }

        [JsonIgnore]
        private string sendPlainTextOnly
        {
            get { return GetStringParameter("sendplaintextonly"); }
            set { SetStringParameter("sendplaintextonly", value); }
        }

        public Dictionary<string, string> LanguagesSupported { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public string resendDrafts
        {
            get { return GetStringParameter("resenddrafts"); }
            set { SetStringParameter("resenddrafts", value); }
        }

        [JsonIgnore]
        public bool UsePreEdit
        {
            get { return Convert.ToBoolean(usePreEdit); }
            set { usePreEdit = value.ToString(); }
        }

        [JsonIgnore]
        public bool UsePostEdit
        {
            get { return Convert.ToBoolean(usePostEdit); }
            set { usePostEdit = value.ToString(); }
        }

        [JsonIgnore]
        private string usePreEdit
        {
            get { return GetStringParameter("usepreedit"); }
            set { SetStringParameter("usepreedit", value); }
        }

        [JsonIgnore]
        private string usePostEdit
        {
            get { return GetStringParameter("usepostedit"); }
            set { SetStringParameter("usepostedit", value); }
        }

        [JsonIgnore]
        public string PreLookupFilename
        {
            get { return GetStringParameter("prelookupfilename"); }
            set { SetStringParameter("prelookupfilename", value); }
        }

        [JsonIgnore]
        public string PostLookupFilename
        {
            get { return GetStringParameter("postlookupfilename"); }
            set { SetStringParameter("postlookupfilename", value); }
        }

        public enum AWSAuthType
        {
            AccessKeys = 1,
            Profile = 2,
            None = 0
        }

        public static string GetAuthTypeDescription(AWSAuthType type)
        {
            if (type == AWSAuthType.AccessKeys)
                return MtProviderConfDialogResources.ComboAuthTypeKeys;
            else if (type == AWSAuthType.Profile)
                return MtProviderConfDialogResources.ComboAuthTypeProfile;
            else
                return "";
        }

        public static AWSAuthType GetAuthType(string typeString)
        {
            if (typeString == null)
                return AWSAuthType.None;
            else if (typeString.Equals(AWSAuthType.AccessKeys.ToString()))
                return AWSAuthType.AccessKeys;
            else if (typeString.Equals(AWSAuthType.Profile.ToString()))
                return AWSAuthType.Profile;
            else
                return AWSAuthType.None;
        }

        [JsonIgnore]
        public AWSAuthType SelectedAuthType
        {
            get
            {
                return GetAuthType(GetStringParameter("selectedauthtype"));
            }
            set
            {
                string typestring = value.ToString();
                SetStringParameter("selectedauthtype", typestring);
            }
        }

        [JsonIgnore]
        public string AccessKey
        {
            get { return _accesskey; } //the creds are going to be held in a static variable so we don't have to get it from credential store all the time
            set { _accesskey = value; }
        }

        [JsonIgnore]
        public string SecretKey
        {
            get { return _secretkey; } //the creds are going to be held in a static variable so we don't have to get it from credential store all the time
            set { _secretkey = value; }
        }

        [JsonIgnore]
        public string ProfileName
        {
            get { return GetStringParameter("profilename"); }
            set { SetStringParameter("profilename", value); }
        }

        [JsonIgnore]
        public string RegionName
        {
            get { return GetStringParameter("regionname"); }
            set { SetStringParameter("regionname", value); }
        }

        [JsonIgnore]
        public bool PersistAWSCreds
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool ResendDrafts //we'll access this from other classes..converting to and from string for purposes of our uri setter/getter above
        {
            get { return Convert.ToBoolean(resendDrafts); }
            set { resendDrafts = value.ToString(); }
        }

        [JsonIgnore]
        public bool SendPlainTextOnly //we'll access this from other classes..converting to and from string for purposes of our uri setter/getter above
        {
            get { return Convert.ToBoolean(sendPlainTextOnly); }
            set { sendPlainTextOnly = value.ToString(); }
        }

        private void SetStringParameter(string p, string value)
        {
            _uriBuilder[p] = value;
        }

        private string GetStringParameter(string p)
        {
            string paramString = _uriBuilder[p];
            return paramString;
        }

        [JsonIgnore]
        public Uri Uri
        {
            get
            {
                return _uriBuilder.Uri;
            }
        }

    }


}

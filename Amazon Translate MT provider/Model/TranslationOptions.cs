using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sdl.Community.AmazonTranslateProvider;
using Sdl.Community.AmazonTranslateTradosPlugin.Studio.TranslationProvider;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Model
{
    /// <summary>
    /// This class is used to hold the provider plug-in settings. 
    /// All settings are automatically stored in a URI.
    /// </summary>
    public class TranslationOptions
    {
        readonly TranslationProviderUriBuilder _uriBuilder;
        public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;

        private static string _secretkey;
        private static string _accesskey;

        public TranslationOptions()
        {
            _uriBuilder = new TranslationProviderUriBuilder(TranslationProvider.TranslationProviderScheme);
            Id = Guid.NewGuid().ToString();
        }

        public TranslationOptions(Uri uri)
        {
            _uriBuilder = new TranslationProviderUriBuilder(uri);
        }

        [JsonIgnore]
        private string sendPlainTextOnly
        {
            get { return GetStringParameter("sendplaintextonly"); }
            set { SetStringParameter("sendplaintextonly", value); }
        }

        public Dictionary<string, string> LanguagesSupported { get; set; } = [];

        [JsonIgnore]
        public string resendDrafts
        {
            get { return GetStringParameter("resenddrafts"); }
            set { SetStringParameter("resenddrafts", value); }
        }

        [JsonIgnore]
        public string Id
        {
            get { return GetStringParameter("id"); }
            set { SetStringParameter("id", value); }
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
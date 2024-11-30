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
using Newtonsoft.Json;
using Sdl.Community.MtEnhancedProvider.Helpers;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using static System.Convert;

namespace Sdl.Community.MtEnhancedProvider
{
    /// <summary>
    /// This class is used to hold the provider plug-in settings. 
    /// All settings are automatically stored in a URI.
    /// </summary>
    public class MtTranslationOptions: IMtTranslationOptions
	{
        private string _apiKey;
        private string _clientid;
        const string MsTranslatorString = "Microsoft Translator"; //these strings should not be localized or changed and are therefore hard-coded as constants
		const string MsTranslatorWithPeString = "Microsoft Translator with Private Endpoint"; //these strings should not be localized or changed and are therefore hard-coded as constants
		const string GTranslateString = "Google Translate"; //these strings should not be localized or changed and are therefore hard-coded as constants

        //The translation method affects when/if the plugin gets called by Studio
        public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;

        TranslationProviderUriBuilder _uriBuilder;

        public MtTranslationOptions()
        {
            _uriBuilder = new TranslationProviderUriBuilder(MtTranslationProvider.ListTranslationProviderScheme);
        }

        public MtTranslationOptions(Uri uri)
        {
            _uriBuilder = new TranslationProviderUriBuilder(uri);
        }

		public Dictionary<string,string> LanguagesSupported { get; set; }
        
		[JsonIgnore]
        private string sendPlainTextOnly
        {
            get => GetStringParameter("sendplaintextonly");
			set => SetStringParameter("sendplaintextonly", value);
		}

        [JsonIgnore]
        private string UseCatId
        {
            get => GetStringParameter("usecatid");
	        set => SetStringParameter("usecatid", value);
        }

        [JsonIgnore]
        public string resendDrafts
        {
            get => GetStringParameter("resenddrafts");
	        set => SetStringParameter("resenddrafts", value);
        }

        [JsonIgnore]
        public bool UsePreEdit
		{
			get => ToBoolean(usePreEdit);
			set => usePreEdit = value.ToString();
        }
		[JsonIgnore]
        public bool UsePostEdit
        {
            get => ToBoolean(usePostEdit);
			set => usePostEdit = value.ToString();
		}
		[JsonIgnore]
		public bool BasicCsv
		{
			get => ToBoolean(SimpleCsv);
			set => SimpleCsv = value.ToString();
		}

		[JsonIgnore]
        private string usePreEdit
        {
            get => GetStringParameter("usepreedit");
	        set => SetStringParameter("usepreedit", value);
        }
        [JsonIgnore]
        private string usePostEdit
        {
            get => GetStringParameter("usepostedit");
	        set => SetStringParameter("usepostedit", value);
        }
        [JsonIgnore]
        public string PreLookupFilename
        {
            get => GetStringParameter("prelookupfilename");
	        set => SetStringParameter("prelookupfilename", value);
        }

		public string SimpleCsv
		{
			get => GetStringParameter("basiccsv");
			set => SetStringParameter("basiccsv", value);
		}

		[JsonIgnore]
        public string PostLookupFilename
        {
            get => GetStringParameter("postlookupfilename");
	        set => SetStringParameter("postlookupfilename", value);
        }
		[JsonIgnore]
		public string JsonFilePath
		{
			get => GetStringParameter("jsonfilepath");
			set => SetStringParameter("jsonfilepath", value);
		}
		[JsonIgnore]
		public string ProjectName
		{
			get => GetStringParameter("projectname");
			set => SetStringParameter("projectname", value);
		}

		[JsonIgnore]
		public string GlossaryPath {
			get => GetStringParameter("glossarypath");
			set => SetStringParameter("glossarypath", value);
		}

		[JsonIgnore]
		public string GoogleEngineModel
		{
			get => GetStringParameter("googleenginemodel");
			set => SetStringParameter("googleenginemodel", value);
		}

		[JsonIgnore]
		public string ProjectLocation
		{
			get => GetStringParameter("projectlocation");
			set => SetStringParameter("projectlocation", value);
		}

		public enum ProviderType
        {
            GoogleTranslate = 1,
            MicrosoftTranslator = 2,
			MicrosoftTranslatorWithPe = 3,
            None = 0
        }

        public static string GetProviderTypeDescription(ProviderType type)
        {
	        switch (type)
	        {
		        case ProviderType.GoogleTranslate:
			        return GTranslateString; //these strings should not be localized and are therefore hard-coded
		        case ProviderType.MicrosoftTranslator:
			        return MsTranslatorString; //these strings should not be localized and are therefore hard-coded
				case ProviderType.MicrosoftTranslatorWithPe:
					return MsTranslatorWithPeString; //these strings should not be localized and are therefore hard-coded
			}
	        return "";
        }

        public static ProviderType GetProviderType(string typeString)
        {
	        //we changed the options provider type to use resource strings..but if a user migrates a project to a machine with a different culture then it will be a problem
            //the solution seems to be to not translate the names for 'Google Translate' and 'Microsoft Translator' ...they both leave it untranslated in their documentation in other languages
	        switch (typeString)
	        {
		        case null:
			        return ProviderType.None;
		        case GTranslateString:
			        return ProviderType.GoogleTranslate;
		        case MsTranslatorString:
			        return ProviderType.MicrosoftTranslator;
				case MsTranslatorWithPeString:
					return ProviderType.MicrosoftTranslatorWithPe;
				default:
			        return ProviderType.None;
	        }
        }

        [JsonIgnore]
        public ProviderType SelectedProvider
        {
            get => GetProviderType(GetStringParameter("selectedprovider"));
	        set 
            {
                var typestring = GetProviderTypeDescription(value);
                SetStringParameter("selectedprovider", typestring); 
            }
        }


		[JsonIgnore]
		public Enums.GoogleApiVersion SelectedGoogleVersion
		{
			get => GetProviderGoogleApiVersion(GetStringParameter("selectedgoogleversion"));
			set
			{
				var typestring = GetProviderTypeDescription(value);
				SetStringParameter("selectedgoogleversion", typestring);
			}
		}

		public static string GetProviderTypeDescription(Enums.GoogleApiVersion googleVersion)
		{
			switch (googleVersion)
			{
				case Enums.GoogleApiVersion.V2:
					return "V2"; 
				case Enums.GoogleApiVersion.V3:
					return "V3";
			}
			return "V2";
		}

		public static Enums.GoogleApiVersion GetProviderGoogleApiVersion(string version)
		{
			switch (version)
			{
				case "V2":
					return Enums.GoogleApiVersion.V2;
				case "V3":
					return Enums.GoogleApiVersion.V3;
				default:
					return Enums.GoogleApiVersion.V2;
			}
		}

		[JsonIgnore]
		//User for Google authentication
		//The apiKey is going to be held in a static variable so we don't have to get it from credential store all the time
		public string ApiKey 
		{
			get => _apiKey;
			set => _apiKey = value;
		}

		// The Microsoft private endpoint url
		public string PeUrl
		{
			get => GetStringParameter("peurl");
			set => SetStringParameter("peurl", value);
		}

		[JsonIgnore] 
		//User for Microsoft authentication
        public string ClientId 
		{
			get => _clientid;
            set => _clientid = value;
		}

		[JsonIgnore]
		public string Region
		{
			get => GetStringParameter("region");
			set => SetStringParameter("region", value);
		}

		[JsonIgnore]
        public bool PersistGoogleKey
        {
            get;
            set;
        }
        [JsonIgnore]
        public bool PersistMicrosoftCreds
        {
            get;
            set;
        }
        [JsonIgnore]
        public bool ResendDrafts //we'll access this from other classes..converting to and from string for purposes of our uri setter/getter above
        {
            get => ToBoolean(resendDrafts);
	        set => resendDrafts = value.ToString();
        }
        [JsonIgnore]
        public bool SendPlainTextOnly //we'll access this from other classes..converting to and from string for purposes of our uri setter/getter above
        {
            get => ToBoolean(sendPlainTextOnly);
	        set => sendPlainTextOnly = value.ToString();
        }
        [JsonIgnore]
        public bool UseCatID //we'll access this from other classes..converting to and from string for purposes of our uri setter/getter above
        {
            get => ToBoolean(UseCatId);
	        set => UseCatId = value.ToString();
        }
        [JsonIgnore]
        public string CatId
        {
            get => GetStringParameter("catid");
	        set => SetStringParameter("catid", value);
        }

        private void SetStringParameter(string p, string value)
        {
            _uriBuilder[p] = value;
        }

        private string GetStringParameter(string p)
        {
            var paramString = _uriBuilder[p];
            return paramString;
        }

        [JsonIgnore]
        public Uri Uri => _uriBuilder.Uri;
	}
}

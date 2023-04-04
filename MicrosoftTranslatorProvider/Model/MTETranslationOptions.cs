using System;
using System.Collections.Generic;
using MicrosoftTranslatorProvider.Interfaces;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using static System.Convert;

namespace MicrosoftTranslatorProvider.Model
{
	public class MTETranslationOptions : ITranslationOptions
	{
		public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;
		private readonly TranslationProviderUriBuilder _uriBuilder;
		private string _clientID;
		private string _privateEndpoint;

		public MTETranslationOptions()
		{
			_uriBuilder = new TranslationProviderUriBuilder(Constants.MicrosoftProviderScheme);
		}

		public MTETranslationOptions(Uri uri)
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}

		public Dictionary<string, string> LanguagesSupported { get; set; }

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
		private string useCustomProviderName
		{
			get => GetStringParameter("useCustomProviderName");
			set => SetStringParameter("useCustomProviderName", value);
		}
		[JsonIgnore]

		public string PreLookupFilename
		{
			get => GetStringParameter("prelookupfilename");
			set => SetStringParameter("prelookupfilename", value);
		}

		[JsonIgnore]
		public string PostLookupFilename
		{
			get => GetStringParameter("postlookupfilename");
			set => SetStringParameter("postlookupfilename", value);
		}

		[JsonIgnore]
		public string ProjectName
		{
			get => GetStringParameter("projectname");
			set => SetStringParameter("projectname", value);
		}

		[JsonIgnore]
		public string ProjectLocation
		{
			get => GetStringParameter("projectlocation");
			set => SetStringParameter("projectlocation", value);
		}

		public string CustomProviderName
		{
			get => GetStringParameter("customProviderName");
			set => SetStringParameter("customProviderName", value);
		}

		public enum ProviderType
		{
			MicrosoftTranslator = 1,
			None = 0
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
		public string ClientID
		{
			get => _clientID;
			set => _clientID = value;
		}

		[JsonIgnore]
		public string Region
		{
			get => GetStringParameter("region");
			set => SetStringParameter("region", value);
		}

		[JsonIgnore]
		public bool PersistMicrosoftCredentials
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

		public bool UseCustomProviderName
		{
			get => ToBoolean(useCustomProviderName);
			set => useCustomProviderName = value.ToString();
		}

		[JsonIgnore]
		public bool SendPlainTextOnly //we'll access this from other classes..converting to and from string for purposes of our uri setter/getter above
		{
			get => ToBoolean(sendPlainTextOnly);
			set => sendPlainTextOnly = value.ToString();
		}

		[JsonIgnore]
		public bool UseCategoryID //we'll access this from other classes..converting to and from string for purposes of our uri setter/getter above
		{
			get => ToBoolean(UseCatId);
			set => UseCatId = value.ToString();
		}

		[JsonIgnore]
		public string CategoryID
		{
			get => GetStringParameter("catid");
			set => SetStringParameter("catid", value);
		}

		[JsonIgnore]
		public bool PersistPrivateEndpoint { get; set; }

		[JsonIgnore]
		public string PrivateEndpoint
		{
			get => _privateEndpoint;
			set => _privateEndpoint = value;
		}

		[JsonIgnore]
		public bool UsePrivateEndpoint
		{
			get => !string.IsNullOrEmpty(PrivateEndpoint);
		}

		[JsonIgnore]
		public Uri Uri => _uriBuilder.Uri;

		private void SetStringParameter(string p, string value)
		{
			_uriBuilder[p] = value;
		}

		private string GetStringParameter(string p)
		{
			var paramString = _uriBuilder[p];
			return paramString;
		}

		public static string GetProviderTypeDescription(ProviderType type)
		{
			return type == ProviderType.MicrosoftTranslator ? Constants.MicrosoftTranslatorString : string.Empty;
		}

		public static ProviderType GetProviderType(string typeString)
		{
			//we changed the options provider type to use resource strings..but if a user migrates a project to a machine with a different culture then it will be a problem
			//the solution seems to be to not translate the names for 'Google Translate' and 'Microsoft Translator'
			//...they both leave it untranslated in their documentation in other languages
			if (typeString is null)
			{
				return ProviderType.MicrosoftTranslator;
			}
			return typeString.Equals(Constants.MicrosoftTranslatorString) ? ProviderType.MicrosoftTranslator : ProviderType.None;
		}
	}
}
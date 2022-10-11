using System;
using System.Collections.Generic;
using MTEnhancedMicrosoftProvider.Interfaces;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using static System.Convert;

namespace MTEnhancedMicrosoftProvider.Studio.TranslationProvider
{
	public class MTEMicrosoftTranslationOptions : ITranslationOptions
	{
		private const string MsTranslatorString = "Microsoft Translator"; //these strings should not be localized or changed and are therefore hard-coded as constants

		private string _clientid;
		private readonly TranslationProviderUriBuilder _uriBuilder;
		public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;

		public MTEMicrosoftTranslationOptions()
		{
			_uriBuilder = new TranslationProviderUriBuilder(MTEMicrosoftProvider.ListTranslationProviderScheme);
		}

		public MTEMicrosoftTranslationOptions(Uri uri)
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
		public string GlossaryPath
		{
			get => GetStringParameter("glossarypath");
			set => SetStringParameter("glossarypath", value);
		}

		[JsonIgnore]
		public string ProjectLocation
		{
			get => GetStringParameter("projectlocation");
			set => SetStringParameter("projectlocation", value);
		}

		public enum ProviderType
		{
			MicrosoftTranslator = 1,
			None = 0
		}

		public static string GetProviderTypeDescription(ProviderType type)
		{
			switch (type)
			{
				case ProviderType.MicrosoftTranslator:
					return MsTranslatorString; //these strings should not be localized and are therefore hard-coded
				default:
					return "";
			}
		}

		public static ProviderType GetProviderType(string typeString)
		{
			//we changed the options provider type to use resource strings..but if a user migrates a project to a machine with a different culture then it will be a problem
			//the solution seems to be to not translate the names for 'Google Translate' and 'Microsoft Translator' ...they both leave it untranslated in their documentation in other languages
			switch (typeString)
			{
				case MsTranslatorString:
					return ProviderType.MicrosoftTranslator;
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
	}
}
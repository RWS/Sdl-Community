using System;
using System.Collections.Generic;
using GoogleTranslatorProvider.Interfaces;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using static System.Convert;

namespace GoogleTranslatorProvider.Models
{
	public class GTPTranslationOptions : ITranslationOptions
	{
		private readonly TranslationProviderUriBuilder _uriBuilder;
		public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;
		private string _apiKey;

		public GTPTranslationOptions(Uri uri = null)
		{
			_uriBuilder = uri is null ? new TranslationProviderUriBuilder(Constants.GoogleTranslationScheme)
									  : new TranslationProviderUriBuilder(uri);
		}

		public Dictionary<string, string> LanguagesSupported { get; set; }

		[JsonIgnore]
		private string sendPlainTextOnly
		{
			get => GetStringParameter("sendplaintextonly");
			set => SetStringParameter("sendplaintextonly", value);
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

		public static string GetProviderTypeDescription(ProviderType type)
		{
			return type switch
			{
				ProviderType.GoogleTranslate => Constants.GoogleTranslatorString,
				_ => string.Empty
			};
		}

		public static ProviderType GetProviderType(string typeString)
		{
			return typeString switch
			{
				Constants.GoogleTranslatorString => ProviderType.GoogleTranslate,
				_ => ProviderType.None
			};
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
		public ApiVersion SelectedGoogleVersion
		{
			get => GetProviderGoogleApiVersion(GetStringParameter("selectedgoogleversion"));
			set
			{
				var typestring = GetProviderTypeDescription(value);
				SetStringParameter("selectedgoogleversion", typestring);
			}
		}

		public static string GetProviderTypeDescription(ApiVersion googleVersion)
		{
			switch (googleVersion)
			{
				case ApiVersion.V2:
					return "V2";
				case ApiVersion.V3:
					return "V3";
			}
			return "V2";
		}

		public static ApiVersion GetProviderGoogleApiVersion(string version)
		{
			switch (version)
			{
				case "V3":
					return ApiVersion.V3;
				default:
					return ApiVersion.V2;
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

		ApiVersion ITranslationOptions.SelectedGoogleVersion
		{
			get => GetProviderGoogleApiVersion(GetStringParameter("selectedgoogleversion"));
			set
			{
				var typestring = GetProviderTypeDescription(value);
				SetStringParameter("selectedgoogleversion", typestring);
			}
		}
	}
}
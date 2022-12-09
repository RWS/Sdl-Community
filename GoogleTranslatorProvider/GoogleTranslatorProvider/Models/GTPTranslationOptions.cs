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
		private string _apiKey;

		public GTPTranslationOptions(Uri uri = null)
		{
			_uriBuilder = uri is null ? new TranslationProviderUriBuilder(Constants.GoogleTranslationScheme)
									  : new TranslationProviderUriBuilder(uri);
		}

		[JsonIgnore]
		public Dictionary<string, string> LanguagesSupported { get; set; }

		[JsonIgnore]
		public bool PersistGoogleKey { get; set; }

		[JsonIgnore]
		public bool SendPlainTextOnly
		{
			get => ToBoolean(GetStringParameter("sendplaintextonly"));
			set => SetStringParameter("sendplaintextonly", value.ToString());
		}

		[JsonIgnore]
		public bool ResendDrafts
		{
			get => ToBoolean(GetStringParameter("resenddrafts")); 
			set => SetStringParameter("resenddrafts", value.ToString());
		}

		[JsonIgnore]
		public bool UsePostEdit
		{
			get => ToBoolean(GetStringParameter("usepostedit"));
			set => SetStringParameter("usepostedit", value.ToString());
		}

		[JsonIgnore]
		public bool UsePreEdit
		{
			get => ToBoolean(GetStringParameter("usepreedit"));
			set => SetStringParameter("usepreedit", value.ToString());
		}

		[JsonIgnore]
		public bool BasicCsv
		{
			get => ToBoolean(GetStringParameter("basiccsv"));
			set => SetStringParameter("basiccsv", value.ToString());
		}

		[JsonIgnore]
		public string PostLookupFilename
		{
			get => GetStringParameter("postlookupfilename");
			set => SetStringParameter("postlookupfilename", value);
		}

		[JsonIgnore]
		public string PreLookupFilename
		{
			get => GetStringParameter("prelookupfilename");
			set => SetStringParameter("prelookupfilename", value);
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

		[JsonIgnore]
		public string JsonFilePath
		{
			get => GetStringParameter("jsonfilepath");
			set => SetStringParameter("jsonfilepath", value);
		}

		[JsonIgnore]
		public string GlossaryPath
		{
			get => GetStringParameter("glossarypath");
			set => SetStringParameter("glossarypath", value);
		}

		[JsonIgnore]
		public string ProjectId
		{
			get => GetStringParameter("projectid");
			set => SetStringParameter("projectid", value);
		}

		[JsonIgnore]
		public string ApiKey
		{
			get => _apiKey;
			set => _apiKey = value;
		}

		[JsonIgnore]
		public Uri Uri => _uriBuilder.Uri;

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
			set => SetStringParameter("selectedgoogleversion", GetProviderTypeDescription(value));
		}


		private string GetProviderTypeDescription(ProviderType type)
		{
			return type switch
			{
				ProviderType.GoogleTranslate => Constants.GoogleTranslatorString,
				_ => string.Empty
			};
		}

		private static ProviderType GetProviderType(string typeString)
		{
			return typeString switch
			{
				Constants.GoogleTranslatorString => ProviderType.GoogleTranslate,
				_ => ProviderType.None
			};
		}

		private static string GetProviderTypeDescription(ApiVersion googleVersion)
		{
			return googleVersion switch
			{
				ApiVersion.V3 => "V3",
				_ => "V2",
			};
		}

		private static ApiVersion GetProviderGoogleApiVersion(string version)
		{
			return version switch
			{
				"V3" => ApiVersion.V3,
				_ => ApiVersion.V2,
			};
		}

		private void SetStringParameter(string p, string value)
		{
			_uriBuilder[p] = value;
		}

		private string GetStringParameter(string p)
		{
			return _uriBuilder[p];
		}
	}
}
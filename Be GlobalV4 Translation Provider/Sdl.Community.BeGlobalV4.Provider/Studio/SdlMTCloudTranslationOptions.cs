using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	public class SdlMTCloudTranslationOptions
	{
		private readonly TranslationProviderUriBuilder _uriBuilder;

		public SdlMTCloudTranslationOptions()
		{
			_uriBuilder = new TranslationProviderUriBuilder(Constants.MTCloudUriScheme);
		}

		public SdlMTCloudTranslationOptions(Uri uri)
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}

		public string ClientId { get; set; }

		public string ClientSecret { get; set; }

		public string AuthenticationMethod { get; set; }

		[JsonIgnore]
		public SdlMTCloudTranslator BeGlobalService { get; set; }

		public string DisplayName { get; set; }

		public Dictionary<string, string> LanguagesSupported { get; set; } = new Dictionary<string, string>();

		public bool ResendDrafts
		{
			get => ResendDraftsParameter != null && Convert.ToBoolean(ResendDraftsParameter);
			set => ResendDraftsParameter = value.ToString();
		}

		[JsonIgnore]
		public string ResendDraftsParameter
		{
			get => GetStringParameter("resenddrafts");
			set => SetStringParameter("resenddrafts", value);
		}
		
		[JsonIgnore]
		public SubscriptionInfo SubscriptionInfo { get; set; }

		[JsonIgnore]
		public Uri Uri => _uriBuilder.Uri;

		private string GetStringParameter(string p)
		{
			var paramString = _uriBuilder[p];
			return paramString;
		}

		private void SetStringParameter(string p, string value)
		{
			_uriBuilder[p] = value;
		}
	}
}
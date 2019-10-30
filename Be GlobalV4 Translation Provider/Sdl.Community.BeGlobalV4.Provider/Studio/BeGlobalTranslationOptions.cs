using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class BeGlobalTranslationOptions
	{
		private readonly TranslationProviderUriBuilder _uriBuilder;

		public BeGlobalTranslationOptions()
		{
			_uriBuilder = new TranslationProviderUriBuilder("sdlmachinetranslationcloudprovider");
		}

		public BeGlobalTranslationOptions(Uri uri)
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}

		public string DisplayName { get; set; }

		[JsonIgnore]
		public string ClientId { get; set; }

		[JsonIgnore]
		public string ClientSecret { get; set; }

		[JsonIgnore]
		public string AuthenticationMethod
		{
			get => GetStringParameter("authenticationMethod");
			set => SetStringParameter("authenticationMethod", value);
		}

		public Dictionary<string, string> LanguagesSupported { get; set; }

		[JsonIgnore]
		public BeGlobalV4Translator BeGlobalService { get; set; }

		[JsonIgnore]
		public string Model
		{
			get => GetStringParameter("model");
			set => SetStringParameter("model", value);
		}

		[JsonIgnore]
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
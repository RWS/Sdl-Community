using System;
using System.Collections.Generic;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class BeGlobalTranslationOptions
	{
		public BeGlobalTranslationOptions()
		{
			_uriBuilder = new TranslationProviderUriBuilder("beglobaltranslationprovider");
		}
		public BeGlobalTranslationOptions(Uri uri)
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}

		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public List<SubscriptionInfo> SubscriptionInfo { get; set; }
		
		public Uri Uri => _uriBuilder.Uri;		 
		private readonly TranslationProviderUriBuilder _uriBuilder;

		public string Model
		{
			get => GetStringParameter("model");
			set => SetStringParameter("model", value);
		}

		public string ResendDraftsParameter
		{
			get => GetStringParameter("resenddrafts");
			set => SetStringParameter("resenddrafts", value);
		}
		public bool ResendDrafts
		{
			get => ResendDraftsParameter != null && Convert.ToBoolean(ResendDraftsParameter);
			set => ResendDraftsParameter = value.ToString();
		}
		public string UseClientAuthenticationParameter
		{
			get => GetStringParameter("useClient");
			set => SetStringParameter("useClient", value);
		}
		public bool UseClientAuthentication
		{
			get => UseClientAuthenticationParameter != null && Convert.ToBoolean(UseClientAuthenticationParameter);
			set => UseClientAuthenticationParameter = value.ToString();
		}

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

using System;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	public class SdlMTCloudTranslationOptions
	{
		private readonly TranslationProviderUriBuilder _uriBuilder;

		public SdlMTCloudTranslationOptions(): this(new Uri($"{Constants.MTCloudUriScheme}://")) { }

		public SdlMTCloudTranslationOptions(Uri uri)
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);			
		}

		public bool ResendDraft
		{
			get => ResendDraftsParameter != null && Convert.ToBoolean(ResendDraftsParameter);
			set => ResendDraftsParameter = value.ToString();
		}


		[JsonIgnore]
		public string ResendDraftsParameter
		{
			get => GetStringParameter("ResendDraft");
			set => SetStringParameter("ResendDraft", value);
		}

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
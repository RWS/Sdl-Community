using System;
using System.Collections.Generic;
using LanguageWeaverProvider.Model.Interface;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider.Model.Options
{
	public class TranslationOptions : ITranslationOptions
	{
		private readonly TranslationProviderUriBuilder _uriBuilder;

		public TranslationOptions(Uri uri = null)
		{
			_uriBuilder = new TranslationProviderUriBuilder(Constants.TranslationScheme);
		}

		public Uri Uri => _uriBuilder.Uri;

		public ProviderSettings ProviderSettings { get; set; }

		public AuthenticationType AuthenticationType { get; set; }

		public PluginVersion Version { get; set; }

		[JsonIgnore]
		public CloudCredentials CloudCredentials { get; set; }

		public List<PairMapping> PairMappings { get; set; }

		public AccessToken AccessToken { get; set; }
	}
}
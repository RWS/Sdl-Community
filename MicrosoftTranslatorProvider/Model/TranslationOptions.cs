using MicrosoftTranslatorProvider.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MicrosoftTranslatorProvider.Model
{
    public class TranslationOptions : ITranslationOptions
	{
		public TranslationOptions(bool isNewInstance = false)
		{
			if (isNewInstance)
			{
				Id = Guid.NewGuid().ToString();
				ProviderSettings = new();
			}
		}

		public string Id { get; set; }

        public string ProviderName => GetProviderName();

        public List<UrlMetadata> Parameters { get; set; }

		public Uri Uri { get; set; }

		public List<PairMapping> PairMappings { get; set; }

		public List<PairModel> PairModels { get; set; }

		[JsonIgnore]
		public MicrosoftCredentials MicrosoftCredentials { get; set; }

		public  AuthenticationType AuthenticationType { get; set; }

		public ProviderSettings ProviderSettings { get; set; }

		[JsonIgnore]
		public PrivateEndpoint PrivateEndpoint { get; set; }

		internal void UpdateUri()
		{
			var uri = AuthenticationType == AuthenticationType.PrivateEndpoint
					? Constants.MicrosoftProviderPrivateEndpointFullScheme
					: Constants.MicrosoftProviderFullScheme;
			Uri = new Uri(uri);
		}

        private string GetProviderName()
        {
			var providerNamePrefix = Constants.MicrosoftNaming_FullName;
            return ProviderSettings?.UseCustomName == true
                 ? $"{providerNamePrefix} - {ProviderSettings.CustomName}"
                 : providerNamePrefix;
        }
    }
}
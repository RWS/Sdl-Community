using System;
using System.Collections.Generic;
using MicrosoftTranslatorProvider.Model;
using TradosProxySettings.Model;

namespace MicrosoftTranslatorProvider.Interfaces
{
	public interface ITranslationOptions
	{
		string Id { get; set; }

        string ProviderName { get; }

        List<UrlMetadata> Parameters { get; set; }

		Uri Uri { get; }

		List<PairModel> PairModels { get; set; }

		List<PairMapping> PairMappings { get; set; }

		MicrosoftCredentials MicrosoftCredentials { get; set; }

		AuthenticationType AuthenticationType { get; set; }

		ProviderSettings ProviderSettings { get; set; }

		PrivateEndpoint PrivateEndpoint { get; set; }

		ProxySettings ProxySettings { get; set; }

	}
}
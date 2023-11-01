using System;
using System.Collections.Generic;

namespace LanguageWeaverProvider.Model.Interface
{
	public interface ITranslationOptions
	{
		Uri Uri { get; }

		ProviderSettings ProviderSettings { get; set; }

		AuthenticationType AuthenticationType { get; set; }

		PluginVersion Version { get; set; }

		CloudCredentials CloudCredentials { get; set; }

		EdgeCredentials EdgeCredentials { get; set; }

		List<PairMapping> PairMappings { get; set; }

		AccessToken AccessToken { get; set; }

		void UpdateUri();

		void UpdateUri(Uri uri);

		void UpdateUri(string uriString);

		void UpdateUri(PluginVersion pluginVersion);
	}
}
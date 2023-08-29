using System;
using System.Collections.Generic;
using LanguageWeaverProvider.Model.Interface;

namespace LanguageWeaverProvider.Model.Options
{
	public class TranslationOptions : ITranslationOptions
	{
		public Uri Uri => new(Constants.TranslationFullScheme);

		public ProviderSettings ProviderSettings { get; set; }

		public AuthenticationType AuthenticationType { get; set; }

		public PluginVersion Version { get; set; }

		public CloudCredentials CloudCredentials { get; set; }

		public List<PairMapping> PairMappings { get; set; }
	}
}
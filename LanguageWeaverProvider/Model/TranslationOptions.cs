using System;
using System.Collections.Generic;
using LanguageWeaverProvider.Model.Interface;

namespace LanguageWeaverProvider.Model.Options
{
	public class TranslationOptions : ITranslationOptions
	{
		public CloudCredentials CloudCredentials { get; set; }

		public AuthenticationType AuthenticationType { get; set; }

		public List<PairMapping> PairMappings { get; set; }

		public Uri Uri => new(Constants.TranslationFullScheme);
		
		public PluginVersion Version { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace LanguageWeaverProvider.Model.Interface
{
	public interface ITranslationOptions
	{
		CloudCredentials CloudCredentials { get; set; }

		PluginVersion Version { get; set; }

		AuthenticationType AuthenticationType { get; set; }

		List<PairMapping> PairMappings { get; set; }

		Uri Uri { get; }
	}
}
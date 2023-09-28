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

		List<PairMapping> PairMappings { get; set; }

		List<RatedSegment> RatedSegments { get; set; }
	}
}
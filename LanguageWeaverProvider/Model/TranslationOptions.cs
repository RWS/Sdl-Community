using System;
using System.Collections.Generic;
using LanguageWeaverProvider.Model.Interface;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Model.Options
{
	public class TranslationOptions : ITranslationOptions
	{
		public Uri Uri { get; set; }

		public ProviderSettings ProviderSettings { get; set; }

		public AuthenticationType AuthenticationType { get; set; }

		public PluginVersion PluginVersion { get; set; }

		[JsonIgnore]
		public CloudCredentials CloudCredentials { get; set; }

		[JsonIgnore]
		public EdgeCredentials EdgeCredentials { get; set; }

		public List<PairMapping> PairMappings { get; set; }

		public AccessToken AccessToken { get; set; }

		public void UpdateUri(string uriString)
		{
			Uri = new Uri(uriString);
		}

		public void UpdateUri(Uri uri)
		{
			UpdateUri(uri.AbsoluteUri);
		}

		public void UpdateUri()
		{
			UpdateUri(PluginVersion);
		}

		public void UpdateUri(PluginVersion pluginVersion)
		{
			switch (pluginVersion)
			{
				case PluginVersion.LanguageWeaverCloud:
					UpdateUri(Constants.CloudFullScheme);
					break;
				case PluginVersion.LanguageWeaverEdge:
					UpdateUri(Constants.EdgeFullScheme);
					break;
				default:
					break;
			}
		}
	}
}
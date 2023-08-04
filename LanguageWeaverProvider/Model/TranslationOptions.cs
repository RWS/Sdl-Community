using System;
using LanguageWeaverProvider.Model.Interface;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Model.Options
{
	public class TranslationOptions : ITranslationOptions
	{
		public CloudCredentials CloudCredentials { get; set; }

		public AuthenticationType AuthenticationType { get; set; }

		public Uri Uri => new(Constants.TranslationFullScheme);
		
		public PluginVersion Version { get; set; }
	}
}
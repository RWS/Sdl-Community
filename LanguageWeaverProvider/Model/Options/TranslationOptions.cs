using System;
using LanguageWeaverProvider.Model.Options.Interface;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Model.Options
{
	public class TranslationOptions : ITranslationOptions
	{
		[JsonIgnore]
		public CloudCredentials CloudCredentials { get; set; }

		public Uri Uri => new Uri(Constants.TranslationFullScheme);
		
		public PluginVersion Version { get; set; }
	}
}
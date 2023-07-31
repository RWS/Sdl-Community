using System;
using LanguageWeaverProvider.Model.Options.Interface;

namespace LanguageWeaverProvider.Model.Options
{
	public class TranslationOptions : ITranslationOptions
	{
		public CloudCredentials CloudCredentials { get; set; }
		public Uri Uri { get; set; }
		public PluginVersion Version { get; set; }
	}
}
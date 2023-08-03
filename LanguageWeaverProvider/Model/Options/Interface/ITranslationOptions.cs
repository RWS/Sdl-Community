using System;

namespace LanguageWeaverProvider.Model.Options.Interface
{
	public interface ITranslationOptions
	{
		public CloudCredentials CloudCredentials { get; set; }

		public PluginVersion Version { get; set; }

		Uri Uri { get; }
	}
}
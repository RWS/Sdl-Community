using System;

namespace LanguageWeaverProvider.Model.Interface
{
	public interface ITranslationOptions
	{
		CloudCredentials CloudCredentials { get; set; }

		PluginVersion Version { get; set; }

		AuthenticationType AuthenticationType { get; set; }

		Uri Uri { get; }
	}
}
using System;

namespace LanguageWeaverProvider.Model.Options.Interface
{
	public interface ITranslationOptions
	{
		public PluginVersion Version { get; set; }

		Uri Uri { get; set; }
	}
}
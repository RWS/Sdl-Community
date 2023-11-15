using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Extensions
{
	public static class StringExtensions
	{
		public static string GetPluginName(string translationProviderState)
		{
			var translationOptions = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
			return GetPluginName(translationOptions);
		}

		public static string GetPluginName(ITranslationOptions translationOptions)
		{
			var pluginName = translationOptions.PluginVersion == PluginVersion.LanguageWeaverCloud ? Constants.PluginNameCloud : Constants.PluginNameEdge;
			if (!string.IsNullOrEmpty(translationOptions.ProviderSettings.CustomName) && translationOptions.ProviderSettings.UseCustomName)
			{
				pluginName += $" - {translationOptions.ProviderSettings.CustomName}";
			}

			return pluginName;
		}
	}
}
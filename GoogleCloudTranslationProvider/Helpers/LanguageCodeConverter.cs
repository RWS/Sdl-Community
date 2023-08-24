using System.Linq;
using LanguageMappingProvider.Database;
using Sdl.Core.Globalization;

namespace GoogleCloudTranslationProvider.Helpers
{
	public static class LanguageCodeConverter
	{
		public static string GetLanguageCode(this CultureCode cultureCode, ApiVersion targetVersion)
		{
			var targetDatabase = targetVersion == ApiVersion.V2
							   ? PluginResources.Database_PluginName_V2
							   : PluginResources.Database_PluginName_V3;

			var database = new LanguageMappingDatabase(targetDatabase, null);
			var mappings = database.GetMappedLanguages();

			var cultureCodeName = cultureCode.Name.ToLower();
			var mappedLanguage = mappings.FirstOrDefault(x => x.TradosCode.ToLower().Equals(cultureCodeName));
			return mappedLanguage?.LanguageCode;
		}
	}
}
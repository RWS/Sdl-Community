using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class LanguageResourcesTemplateContainer : ILanguageResourcesContainer
	{
		private readonly FileBasedLanguageResourcesTemplate _languageResourceTemplate;

		public LanguageResourcesTemplateContainer(string filePath)
		{
			_languageResourceTemplate = new FileBasedLanguageResourcesTemplate(filePath);
		}

		public LanguageResourceBundleCollection LanguageResourceBundles =>
			_languageResourceTemplate.LanguageResourceBundles;

		public void Save()
		{
			_languageResourceTemplate.Save();
		}

		public bool ValidateTemplate()
		{
			return LanguageResourceBundles?.Count != 0;
		}
	}
}
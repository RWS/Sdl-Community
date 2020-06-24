using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
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

		public BuiltinRecognizers? Recognizers => _languageResourceTemplate.Recognizers;

		public WordCountFlags? WordCountFlags => _languageResourceTemplate.WordCountFlags;

		public void Save()
		{
			_languageResourceTemplate.Save();
		}

		public bool ValidateTemplate(bool isImport)
		{
			return isImport ? LanguageResourceBundles != null : LanguageResourceBundles?.Count != 0;
		}
	}
}
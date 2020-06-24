using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models.Interfaces
{
	public interface ILanguageResourcesContainer
	{
		LanguageResourceBundleCollection LanguageResourceBundles { get; }
		BuiltinRecognizers? Recognizers { get; }
		WordCountFlags? WordCountFlags { get; }

		void Save();

		bool ValidateTemplate(bool isImport);
	}
}
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models.Interfaces
{
	public interface ILanguageResourcesAdapter : ILanguageResourcesContainer
	{
		void Load(string filePath);
	}
}
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public interface IResource
	{
		void AddLanguageResourceToBundle(LanguageResourceBundle langResBundle);
	}
}
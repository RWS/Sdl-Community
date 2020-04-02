using Sdl.Community.MTCloud.Provider.Studio;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ILanguageMappingsService
	{
		LanguageMappingSettings GetLanguageMappingSettings();

		void RemoveLanguageMappingSettings();

		void SaveLanguageMappingSettings(LanguageMappingSettings settings);
	}
}

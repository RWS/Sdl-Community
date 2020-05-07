using Sdl.Community.IATETerminologyProvider.Model;

namespace Sdl.Community.IATETerminologyProvider.Interface
{
    public interface IIateSettingsService
    {
	    ProviderSettings GetProviderSettings();

	    void RemoveProviderSettings();

	    void SaveProviderSettings(ProviderSettings settings);
    }
}

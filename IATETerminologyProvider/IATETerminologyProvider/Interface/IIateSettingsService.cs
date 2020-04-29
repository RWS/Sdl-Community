using IATETerminologyProvider.Model;

namespace IATETerminologyProvider.Interface
{
    public interface IIateSettingsService
    {
	    ProviderSettings GetProviderSettings();

	    void RemoveProviderSettings();

	    void SaveProviderSettings(ProviderSettings settings);
    }
}

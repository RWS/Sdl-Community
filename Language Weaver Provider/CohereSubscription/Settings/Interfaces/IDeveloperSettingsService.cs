using LanguageWeaverProvider.CohereSubscription.Settings.Model;

namespace LanguageWeaverProvider.CohereSubscription.Settings.Interfaces
{
    public interface IDeveloperSettingsService
    {
        DeveloperSettings LoadCreatingDefaultsIfMissing();

        DeveloperSettings ApplyToTheCurrentEnvironment();
    }
}

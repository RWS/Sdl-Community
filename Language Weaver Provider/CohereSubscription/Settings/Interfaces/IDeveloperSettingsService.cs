using LanguageWeaverProvider.CohereSubscription.Settings.Model;

namespace LanguageWeaverProvider.CohereSubscription.Settings.Interfaces
{
    public interface IDeveloperSettingsService
    {
        /// <summary>
        /// Reads the developer settings, writing the file with production defaults if it is not there yet.
        /// Always returns a fully populated instance.
        /// </summary>
        DeveloperSettings Load();

        /// <summary>
        /// Applies the settings that have to take effect before anything reads them - currently the Cloud
        /// environment, which is fixed for the lifetime of the process once sign-in has used it.
        /// </summary>
        DeveloperSettings Apply();
    }
}

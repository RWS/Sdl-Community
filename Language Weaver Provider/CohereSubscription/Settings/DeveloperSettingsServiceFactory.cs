using LanguageWeaverProvider.CohereSubscription.Settings.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Settings.Services;
using LanguageWeaverProvider.Model;

namespace LanguageWeaverProvider.CohereSubscription.Settings
{
    public class DeveloperSettingsServiceFactory
    {
        public static IDeveloperSettingsService Create()
        {
            var pathInfo = new PathInfo();
            var storageService = new FileStorageService();

            return new DeveloperSettingsService(pathInfo, storageService);
        }
    }
}

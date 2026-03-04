using LanguageWeaverProvider.CohereSubscription.Settings.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Settings.Services;
using LanguageWeaverProvider.Model;

namespace LanguageWeaverProvider.CohereSubscription.Settings
{
    public class CohereSubscriptionSettingsServiceFactory
    {
        public static ICohereSubscriptionSettingsService Create()
        {
            var pathInfo = new PathInfo();
            var storageService = new FileStorageService();

            return new CohereSubscriptionSettingsService(pathInfo, storageService);
        }
    }
}

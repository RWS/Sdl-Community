using LanguageWeaverProvider.CohereSubscription.Settings.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Settings.Model;
using LanguageWeaverProvider.Model.Interface;
using System;

namespace LanguageWeaverProvider.CohereSubscription.Settings.Services
{
    public class CohereSubscriptionSettingsService : ICohereSubscriptionSettingsService
    {
        private readonly IPathInfo _pathInfo;
        private readonly IStorageService _storageService;

        public CohereSubscriptionSettingsService(IPathInfo pathInfo, IStorageService storageService)
        {
            _pathInfo = pathInfo;
            _storageService = storageService;
        }

        public bool GetDoNotShowAgain()
        {
            if (!_storageService.Exists(_pathInfo.SettingsPath))
                return false;

            return 
                _storageService.Load<CohereSubscriptionSettings>(_pathInfo.SettingsPath)?.DoNotShowAgain ??
                false;
        }

        public void SetDoNotShowAgain(bool value)
        {
            _storageService.Save<CohereSubscriptionSettings>(
                _pathInfo.SettingsPath,
                new CohereSubscriptionSettings() { DoNotShowAgain = value });
        }
    }
}

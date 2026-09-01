using LanguageWeaverProvider.CohereSubscription.Decision.Services;
using LanguageWeaverProvider.CohereSubscription.Settings.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Settings.Model;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using NLog;
using System;

namespace LanguageWeaverProvider.CohereSubscription.Settings.Services
{
    public class DeveloperSettingsService : IDeveloperSettingsService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IPathInfo _pathInfo;
        private readonly IStorageService _storageService;

        public DeveloperSettingsService(IPathInfo pathInfo, IStorageService storageService)
        {
            _pathInfo = pathInfo;
            _storageService = storageService;
        }

        public DeveloperSettings LoadCreatingDefaultsIfMissing()
        {
            var defaults = new DeveloperSettings
            {
                Environment = CloudEnvironment.Production.Name,
                TrialPromptFromDaysRemaining = CohereSubscriptionDecisionService.DefaultTrialPromptFromDaysRemaining
            };

            try
            {
                if (!_storageService.Exists(_pathInfo.DeveloperSettingsPath))
                {
                    _storageService.Save(_pathInfo.DeveloperSettingsPath, defaults);
                    return defaults;
                }

                var settings = _storageService.Load<DeveloperSettings>(_pathInfo.DeveloperSettingsPath);
                if (settings == null)
                {
                    return defaults;
                }

                return ResolveEachValueIndependently(settings);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "[Cohere] Developer settings could not be read. Using defaults.");
                return defaults;
            }
        }

        private static DeveloperSettings ResolveEachValueIndependently(DeveloperSettings settings)
        {
            settings.Environment = ResolveEnvironment(settings.Environment).Name;

            if (!settings.TrialPromptFromDaysRemaining.HasValue
             || settings.TrialPromptFromDaysRemaining < 0)
            {
                settings.TrialPromptFromDaysRemaining =
                    CohereSubscriptionDecisionService.DefaultTrialPromptFromDaysRemaining;
            }

            return settings;
        }

        public DeveloperSettings ApplyToTheCurrentEnvironment()
        {
            var settings = LoadCreatingDefaultsIfMissing();

            CloudEnvironment.Current = ResolveEnvironment(settings.Environment);
            if (CloudEnvironment.Current != CloudEnvironment.Production)
            {
                Logger.Warn(
                    "[Cohere] Running against {0}, set in {1}.",
                    CloudEnvironment.Current.Name,
                    _pathInfo.DeveloperSettingsPath);
            }

            return settings;
        }

        private static CloudEnvironment ResolveEnvironment(string name)
        {
            if (string.Equals(name, CloudEnvironment.Uat.Name, StringComparison.OrdinalIgnoreCase))
            {
                return CloudEnvironment.Uat;
            }

            return CloudEnvironment.Production;
        }
    }
}

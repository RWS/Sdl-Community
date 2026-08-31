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
        /// <summary>
        /// The shipped behaviour, used both when the file is absent and when a value in it cannot be
        /// understood. Keeping them here rather than at the call sites means a half-filled file still
        /// produces the production journey.
        /// </summary>
        public const int DefaultTrialPromptFromDaysRemaining = 7;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IPathInfo _pathInfo;
        private readonly IStorageService _storageService;

        public DeveloperSettingsService(IPathInfo pathInfo, IStorageService storageService)
        {
            _pathInfo = pathInfo;
            _storageService = storageService;
        }

        public DeveloperSettings Load()
        {
            var defaults = new DeveloperSettings
            {
                Environment = CloudEnvironment.Production.Name,
                TrialPromptFromDaysRemaining = DefaultTrialPromptFromDaysRemaining
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

                // A file written by hand can be missing keys or carry a typo. Each value falls back on its
                // own so that one bad entry does not discard the rest.
                settings.Environment = ResolveEnvironment(settings.Environment).Name;
                if (settings.TrialPromptFromDaysRemaining < 0)
                {
                    settings.TrialPromptFromDaysRemaining = null;
                }

                return settings;
            }
            catch (Exception ex)
            {
                // Settings are a convenience; a locked or unreadable file must never stop Studio loading.
                Logger.Error(ex, "[Cohere] Developer settings could not be read. Using defaults.");
                return defaults;
            }
        }

        public DeveloperSettings Apply()
        {
            var settings = Load();

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

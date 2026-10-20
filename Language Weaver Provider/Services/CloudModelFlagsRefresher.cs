using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using NLog;

namespace LanguageWeaverProvider.Services
{
    /// <summary>
    /// Keeps the server-driven capability flags of saved Cloud pair mappings (<c>qeSupport</c>, adaptation support)
    /// in line with the live language-pairs API. Pair mappings are persisted with the provider, so without this a
    /// capability Language Weaver enables later for a tenant (e.g. MTQE on Pro models) is never requested until the
    /// user clicks "Reset and identify pairs".
    /// </summary>
    public static class CloudModelFlagsRefresher
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);
        private static readonly ConcurrentDictionary<string, DateTime> LastRefreshByProvider = new();

        /// <summary>
        /// Fetches the account's language pairs and updates the cached flags in place. Throttled per provider to
        /// once per <see cref="RefreshInterval"/> unless <paramref name="force"/> is set. Never throws: on any
        /// failure the cached values are kept and the error is logged.
        /// </summary>
        public static async Task RefreshAsync(ITranslationOptions translationOptions, bool force = false)
        {
            if (translationOptions is null
             || translationOptions.PluginVersion != PluginVersion.LanguageWeaverCloud
             || translationOptions.AccessToken is null
             || translationOptions.PairMappings is null
             || !translationOptions.PairMappings.Any())
            {
                return;
            }

            var providerKey = translationOptions.Id ?? string.Empty;
            var now = DateTime.UtcNow;
            if (!force
             && LastRefreshByProvider.TryGetValue(providerKey, out var lastRefresh)
             && now - lastRefresh < RefreshInterval)
            {
                return;
            }

            // Recorded before the call so an unreachable API is not retried on every lookup.
            LastRefreshByProvider[providerKey] = now;

            try
            {
                var accountModels = await CloudService.GetResources<PairModel>(translationOptions.AccessToken, CloudResources.LanguagePairs).ConfigureAwait(false);
                if (ApplyServerFlags(translationOptions.PairMappings, accountModels))
                {
                    Logger.Info("Updated cached model capabilities (qeSupport/adaptation) from the language-pairs API.");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not refresh model capabilities from the language-pairs API; using cached values.");
            }
        }

        /// <summary>
        /// Copies the capability flags from <paramref name="accountModels"/> onto the matching cached models (by
        /// <see cref="PairModel.Name"/>, e.g. "engfra_pro"), including each mapping's <see cref="PairMapping.SelectedModel"/>,
        /// which is a separate instance after deserialization. Models are updated in place so user selections such as
        /// linguistic options are preserved; cached models the API no longer returns, and new API models, are left alone.
        /// </summary>
        /// <returns><c>true</c> if any flag changed.</returns>
        public static bool ApplyServerFlags(IEnumerable<PairMapping> pairMappings, IEnumerable<PairModel> accountModels)
        {
            if (pairMappings is null || accountModels is null)
            {
                return false;
            }

            var liveModels = accountModels
                .Where(model => !string.IsNullOrEmpty(model?.Name))
                .GroupBy(model => model.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

            var changed = false;
            foreach (var pairMapping in pairMappings.Where(mapping => mapping is not null))
            {
                var cachedModels = (pairMapping.Models ?? Enumerable.Empty<PairModel>()).Append(pairMapping.SelectedModel);
                foreach (var cachedModel in cachedModels)
                {
                    if (cachedModel?.Name is null || !liveModels.TryGetValue(cachedModel.Name, out var liveModel))
                    {
                        continue;
                    }

                    changed |= CopyFlags(liveModel, cachedModel);
                }
            }

            return changed;
        }

        private static bool CopyFlags(PairModel source, PairModel target)
        {
            var changed = target.QeSupport != source.QeSupport
                       || target.AdaptationSupport != source.AdaptationSupport
                       || target.AutoAdaptiveSupport != source.AutoAdaptiveSupport;

            target.QeSupport = source.QeSupport;
            target.AdaptationSupport = source.AdaptationSupport;
            target.AutoAdaptiveSupport = source.AutoAdaptiveSupport;
            return changed;
        }
    }
}

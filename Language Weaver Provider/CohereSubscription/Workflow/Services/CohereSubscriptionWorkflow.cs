using LanguageWeaverProvider.CohereSubscription.Workflow.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.CohereSubscription.Workflow.Services
{
    /// <summary>
    /// Resolves the Trados LLM (Language Weaver Pro / Cohere) entitlement for the account the plug-in is
    /// actually configured with, using that provider's own credentials.
    ///
    /// Deliberately does not consult the Trados Studio sign-in. That is a separate identity in a separate id
    /// space — Studio holds a Trados account id, the provider holds a Language Weaver account id — and the two
    /// can belong to different accounts, so an entitlement read through Studio cannot be shown to say anything
    /// about the account the user is translating with.
    ///
    /// Both facts the prompt needs come from data the plug-in already fetches:
    ///   * entitlement — the account's language pairs carry a tier; Pro pairs are typed GENERICPLUS, which is
    ///     what the add-on grants (its trial feature is GENERIC_PLUS_LANGUAGE_PAIRS).
    ///   * role — v4/accounts/users/self returns userRole; the plug-in already calls it for the account id.
    ///
    /// Any failure returns <c>null</c>, which the decision service renders as "no prompt" — the specified
    /// behaviour for an undeterminable entitlement (DET-421, case E): show nothing, log why, retry next startup.
    /// </summary>
    public class CohereSubscriptionWorkflow : ICohereSubscriptionWorkflowService
    {
        private const string ProLanguagePairType = "GENERICPLUS";
        private const string AdminRole = "ADMIN";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<CohereSubscriptionData> ExecuteAsync()
        {
            var translationOptions = GetConfiguredCloudProvider();
            if (translationOptions is null)
            {
                // No Language Weaver Cloud provider is set up (or it has not authenticated yet). Edge providers
                // are excluded on purpose: the add-on is a cloud commerce concept and cannot apply to them.
                Logger.Debug("[Cohere] No authenticated Language Weaver Cloud provider configured; skipping entitlement check.");
                return null;
            }

            try
            {
                var accessToken = translationOptions.AccessToken;
                var languagePairs = await CloudService.GetResources<PairModel>(accessToken, CloudResources.LanguagePairs);
                if (languagePairs is null)
                {
                    Logger.Warn("[Cohere] Could not read the account's language pairs; entitlement is undeterminable.");
                    return null;
                }

                var data = MapEntitlement(languagePairs.Any(IsProLanguagePair), await CloudService.GetUserRole(accessToken));
                Logger.Info(
                    "[Cohere] Entitlement resolved for account {0}: detected={1}, admin={2}.",
                    accessToken.AccountId, data.IsCohereDetected, data.IsAdmin);

                return data;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "[Cohere] Entitlement check failed; no prompt will be shown.");
                return null;
            }
        }

        /// <summary>
        /// Maps the two observable facts onto the prompt's decision inputs.
        ///
        /// Trial state is not represented: nothing currently exposes whether the account is on a trial, when it
        /// started, or when it ends. Both trial flags are therefore left false, which collapses the two trial
        /// cases into the neighbouring ones — an account on trial holds Pro pairs and so reads as paid (silence
        /// rather than "your trial ends in X days"), and an expired trial reads as never-had-it. Neither shows
        /// the user anything untrue; both miss an upsell. See COHERE_CONTEXT.md.
        /// </summary>
        public static CohereSubscriptionData MapEntitlement(bool hasProLanguagePairs, string userRole)
        {
            return new CohereSubscriptionData
            {
                IsCohereDetected = hasProLanguagePairs,
                IsPaid = hasProLanguagePairs,
                IsTrial = false,
                IsTrialExpired = false,
                IsAdmin = AdminRole.Equals(userRole, StringComparison.OrdinalIgnoreCase)
            };
        }

        private static bool IsProLanguagePair(PairModel languagePair)
            => ProLanguagePairType.Equals(languagePair?.Type, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// The Language Weaver Cloud provider whose credentials the entitlement is read with. Picks the first
        /// authenticated one: with several configured the prompt is a single account-level message, so any
        /// authenticated cloud account answers "does this user's organisation have the add-on".
        /// </summary>
        private static ITranslationOptions GetConfiguredCloudProvider()
        {
            return ApplicationInitializer.TranslationOptions?.Values.FirstOrDefault(options =>
                options?.PluginVersion == PluginVersion.LanguageWeaverCloud
             && options.AccessToken?.BaseUri is not null
             && !string.IsNullOrWhiteSpace(options.AccessToken.AccountId));
        }
    }
}

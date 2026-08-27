using LanguageWeaverProvider.CohereSubscription.Workflow.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using LanguageWeaverProvider.Infrastructure.Http.Services;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;
using NLog;
using Sdl.LanguageCloud.IdentityApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.CohereSubscription.Workflow.Services
{
    /// <summary>
    /// Resolves the signed-in user's Language Weaver (Cohere / Trados LLM) entitlement.
    ///
    /// The lookup is two hops, because the entitlement endpoint is keyed on the Account Portal identity
    /// rather than the Trados one:
    ///
    ///   1. <c>GET {lcHost}/lc-api/gw-account-web/accounts/{tradosAccountId}</c> -> <c>businessAccountId</c>
    ///   2. <c>GET {accountPortal}/account-portal/v1/weaver/details/{businessAccountId}</c> -> <see cref="LanguageWeaverDetails"/>
    ///
    /// The details endpoint originally required a <c>recurlyAccountId</c>, which cost a third request to
    /// translate <c>businessAccountId</c> -> <c>recurlyId</c>. That hop was removed on the service side: the
    /// endpoint now accepts <c>businessAccountId</c> directly and returns the same body.
    ///
    /// Every failure path returns <c>null</c>, which the decision service renders as "no pop-up". That is the
    /// specified behaviour for an undeterminable entitlement (DET-421, case E): show the user nothing, record
    /// the reason in the log, and re-evaluate on the next startup.
    /// </summary>
    public class CohereSubscriptionWorkflow : ICohereSubscriptionWorkflowService
    {
        private static string AccountPortalApiBaseUrl => CloudEnvironment.Current.AccountPortalApiBaseUrl;
        private const string AccountPortalDetailsPath = "account-portal/v1/weaver/details/";

        // Documented host for this endpoint. Whether US-region accounts need a different one is still open with
        // the service team, so the request address is logged to make a wrong choice visible in the field.
        private static string LanguageCloudAccountsUrl => CloudEnvironment.Current.LanguageCloudAccountsUrl;

        private const string AdminRole = "ADMIN";
        private const string ProLanguagePairType = "GENERICPLUS";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // One client for the lifetime of the plug-in: a per-call HttpClient leaks a socket per request.
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<CohereSubscriptionData> ExecuteAsync()
        {
            var languageCloudIdentity = LanguageCloudIdentityApi.Instance;
            if (languageCloudIdentity is null
             || string.IsNullOrWhiteSpace(languageCloudIdentity.AccessToken))
            {
                // Not signed in to Language Cloud yet. The startup manager stays subscribed, so this is
                // re-evaluated when the user activates a view after signing in.
                Logger.Debug("[Cohere] No Language Cloud session (no access token); skipping entitlement check.");
                return null;
            }

            var authorizationHeaders = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {languageCloudIdentity.AccessToken}"
            };

            // The Trados sign-in token is accepted by the Language Weaver Cloud API — same Auth0 application
            // and audience — so the user's own Language Weaver account and role resolve from it directly,
            // with no configured provider involved.
            var languageWeaverToken = new AccessToken
            {
                Token = languageCloudIdentity.AccessToken,
                TokenType = "Bearer",
                BaseUri = new Uri(Constants.CloudEUUrl)
            };

            var (accountId, userRole) = await GetSelf(languageWeaverToken);
            languageWeaverToken.AccountId = accountId;
            if (string.IsNullOrWhiteSpace(userRole))
            {
                // Expected for an account that was never provisioned into Language Weaver: users/self answers
                // 403 "user ... does not exist". The role is unknown, not non-admin; see IsAdminOrUnknown.
                Logger.Info("[Cohere] No Language Weaver role for this sign-in; treating the user as an administrator.");
            }

            // The active tenant is only ever needed to look up the Account Portal record. It can legitimately be
            // empty for a valid session (no tenant selected yet), and that is not a reason to abandon the check:
            // the token above already identifies the user, so fall through to the no-trial-history path, which
            // answers from the account's own language pairs instead.
            var (businessAccountId, businessSubscriptionId) = string.IsNullOrWhiteSpace(languageCloudIdentity.ActiveTenantId)
                ? (null, null)
                : await GetAccountPortalIds(languageCloudIdentity.ActiveTenantId, authorizationHeaders);

            if (string.IsNullOrWhiteSpace(businessAccountId))
            {
                // Not a failure. The Account Portal record is created when a trial starts, so its absence
                // means this account has never started one — precisely the state the prompt exists to
                // address. The trial endpoint cannot answer for such an account, but the account's own
                // language pairs still say whether Pro is already held.
                return await ResolveWithoutTrialHistory(languageWeaverToken, userRole);
            }

            var details = await GetLanguageWeaverDetails(businessAccountId, authorizationHeaders);
            if (details is null)
            {
                return null;
            }

            var data = MapDetails(details, userRole, businessAccountId, businessSubscriptionId);
            Logger.Info(
                "[Cohere] Entitlement resolved: detected={0}, paid={1}, trial={2}, trialExpired={3}, admin={4}.",
                data.IsCohereDetected, data.IsPaid, data.IsTrial, data.IsTrialExpired, data.IsAdmin);

            return data;
        }

        /// <summary>
        /// Hop 1: exchanges the Trados account id (the identity API's active tenant) for the Account Portal
        /// <c>businessAccountId</c>, along with the <c>businessSubscriptionId</c>. The account id is
        /// <c>null</c> when the account was not provisioned through Account Portal, since there is then no
        /// entitlement record to read. The subscription id is not used for linking - Account Portal discards a
        /// deep path on external entry - but is carried so it is available without another round trip.
        /// The account id is <c>null</c> when the account was not provisioned through Account Portal, since
        /// there is then no entitlement record to read. The subscription id can be null on its own even for a
        /// provisioned account, which only costs the deep link, not the entitlement check.
        /// </summary>
        private static async Task<(string BusinessAccountId, string BusinessSubscriptionId)> GetAccountPortalIds(
            string tradosAccountId, Dictionary<string, string> headers)
        {
            var requestUri = LanguageCloudAccountsUrl + Uri.EscapeDataString(tradosAccountId);
            Logger.Debug("[Cohere] Resolving businessAccountId via {0}", requestUri);

            var service = new GenericHTTPService<object, LanguageCloudAccountResponse>(
                HttpClient, headers, HttpMethod.Get, requestUri);
            var response = await service.SendRequest(null);

            if (!response.Success || response.Response?.Account is null)
            {
                Logger.Warn(
                    "[Cohere] Could not read the Language Cloud account from {0}: {1}",
                    requestUri, DescribeErrors(response?.Errors));
                return (null, null);
            }

            var businessAccountId = response.Response.Account.BusinessAccountId;
            if (string.IsNullOrWhiteSpace(businessAccountId))
            {
                Logger.Info(
                    "[Cohere] Account has no businessAccountId (not provisioned through Account Portal); no entitlement to evaluate.");
                return (null, null);
            }

            return (businessAccountId, response.Response.Account.BusinessSubscriptionId);
        }

        /// <summary>
        /// Hop 2: reads the Language Weaver entitlement for the given Account Portal business account.
        /// </summary>
        private static async Task<LanguageWeaverDetails> GetLanguageWeaverDetails(
            string businessAccountId, Dictionary<string, string> headers)
        {
            var requestUri = new Uri(
                new Uri(AccountPortalApiBaseUrl),
                AccountPortalDetailsPath + Uri.EscapeDataString(businessAccountId)).AbsoluteUri;

            var service = new GenericHTTPService<object, LanguageWeaverDetails>(
                HttpClient, headers, HttpMethod.Get, requestUri);
            var response = await service.SendRequest(null);

            if (!response.Success || response.Response is null)
            {
                Logger.Warn(
                    "[Cohere] Could not read Language Weaver details from {0}: {1}",
                    requestUri, DescribeErrors(response?.Errors));
                return null;
            }

            return response.Response;
        }

        /// <summary>
        /// Resolves entitlement for an account with no Account Portal record, i.e. one that has never started a
        /// trial. Trial state is knowably absent here rather than unknown, so the only open question is whether
        /// Pro is already held — an account can be provisioned with it without ever trialing. That is answered
        /// from the configured provider's own credentials: the account's language pairs carry a tier, and Pro
        /// pairs are typed GENERICPLUS (the add-on's trial feature is GENERIC_PLUS_LANGUAGE_PAIRS).
        /// </summary>
        private static async Task<CohereSubscriptionData> ResolveWithoutTrialHistory(
            AccessToken languageWeaverToken, string userRole)
        {
            if (string.IsNullOrWhiteSpace(languageWeaverToken.AccountId))
            {
                Logger.Debug("[Cohere] No trial record and no Language Weaver account for this sign-in; entitlement is undeterminable.");
                return null;
            }

            try
            {
                var languagePairs = await CloudService.GetResources<PairModel>(
                    languageWeaverToken, CloudResources.LanguagePairs);
                if (languagePairs is null)
                {
                    Logger.Warn("[Cohere] Could not read the account's language pairs; entitlement is undeterminable.");
                    return null;
                }

                var data = MapEntitlement(languagePairs.Any(IsProLanguagePair), userRole);
                Logger.Info(
                    "[Cohere] No trial record; entitlement resolved for account {0}: detected={1}, admin={2}.",
                    languageWeaverToken.AccountId, data.IsCohereDetected, data.IsAdmin);

                return data;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "[Cohere] Language-pair lookup failed; no prompt will be shown.");
                return null;
            }
        }

        /// <summary>
        /// Maps an account that has never started a trial. Both trial flags are false by knowledge rather than
        /// by ignorance: no Account Portal record exists, and that record is created when a trial starts.
        /// </summary>
        public static CohereSubscriptionData MapEntitlement(bool hasProLanguagePairs, string userRole)
        {
            return new CohereSubscriptionData
            {
                IsCohereDetected = hasProLanguagePairs,
                IsPaid = hasProLanguagePairs,
                IsTrial = false,
                IsTrialExpired = false,
                IsAdmin = IsAdminOrUnknown(userRole)
            };
        }

        /// <summary>
        /// A role that could not be read is unknown, not known-to-be-non-admin. An account that was never
        /// provisioned into Language Weaver — which is every never-trialed account, i.e. exactly the population
        /// the "start a trial" prompt exists for — has no readable role from any source in this flow:
        /// <c>v4/accounts/users/self</c> answers 403 <c>user ... does not exist</c>, and neither the account-web
        /// body nor the sign-in JWT carries a role claim. Treating that as non-admin makes the admin prompt
        /// unreachable for the users it is written for, and tells an account owner to ask an administrator who
        /// does not exist. Only a role that was actually read and is not <c>ADMIN</c> suppresses the admin variant.
        /// </summary>
        private static bool IsAdminOrUnknown(string userRole)
            => string.IsNullOrWhiteSpace(userRole)
            || AdminRole.Equals(userRole, StringComparison.OrdinalIgnoreCase);

        private static bool IsProLanguagePair(PairModel languagePair)
            => ProLanguagePairType.Equals(languagePair?.Type, StringComparison.OrdinalIgnoreCase);


        /// <summary>
        /// Reads the signed-in user's role on the Language Weaver account. The Trados sign-in token is issued
        /// by the same Auth0 application and audience the Language Weaver Cloud API accepts, so the role can be
        /// resolved from the Trados identity alone, with no configured provider involved.
        ///
        /// The account-portal details response carries no role, and neither does the account-web response, so
        /// this is the only known source. A null role is treated as non-admin by <see cref="MapDetails"/>.
        /// </summary>
        private static async Task<(string AccountId, string UserRole)> GetSelf(AccessToken languageWeaverToken)
        {
            try
            {
                return await CloudService.GetSelf(languageWeaverToken);
            }
            catch (Exception ex)
            {
                // Both values are optional: a missing role reads as non-admin, and a missing account id only
                // costs the never-trialed branch its Pro check. Neither is worth aborting the whole check for.
                Logger.Warn(ex, "[Cohere] Could not read the user's Language Weaver account or role.");
                return (null, null);
            }
        }

        private static string DescribeErrors(IEnumerable<string> errors)
            => errors is null ? "no details" : string.Join("; ", errors);

        public static CohereSubscriptionData MapDetails(LanguageWeaverDetails details, string userRole = null, string businessAccountId = null, string businessSubscriptionId = null)
        {
            if (details is null)
            {
                return null;
            }

            // Status values are those documented for the details endpoint: NOT_STARTED, IN_PROGRESS, and the
            // terminal states a started trial ends in. Matching on the literal word "trial" would miss every
            // one of them.
            var trialStatus = (details.TrialStatus ?? string.Empty).Trim();
            var isTrialRunning = trialStatus.Equals("IN_PROGRESS", StringComparison.OrdinalIgnoreCase);
            var isTrialOver = trialStatus.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase)
                           || trialStatus.Equals("EXPIRED", StringComparison.OrdinalIgnoreCase)
                           || trialStatus.Equals("ENDED", StringComparison.OrdinalIgnoreCase);

            return new CohereSubscriptionData
            {
                IsCohereDetected = details.IsProActive || isTrialRunning || isTrialOver,
                // Converting to paid cancels the trial, so an active Pro add-on sits alongside a terminal
                // trial status. Pro therefore decides "paid" on its own: pairing it with the trial state
                // would show a paying customer the "trial expired" prompt.
                IsPaid = details.IsProActive,
                IsTrial = isTrialRunning || isTrialOver,
                IsTrialExpired = isTrialOver,
                // An unreadable role reads as admin, not non-admin: see IsAdminOrUnknown.
                IsAdmin = IsAdminOrUnknown(userRole),
                BusinessAccountId = businessAccountId,
                BusinessSubscriptionId = businessSubscriptionId
            };
        }
    }
}

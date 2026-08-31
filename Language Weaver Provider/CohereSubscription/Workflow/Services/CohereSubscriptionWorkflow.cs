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
    /// Everything is read from Account Portal. The Language Weaver portal is deliberately not consulted:
    /// it is no longer an accepted source for the user's role, so every eligible account is treated as an
    /// administrator until a role source on Account Portal exists.
    ///
    /// The lookup is three hops, because the entitlement endpoint is keyed on the Account Portal identity
    /// rather than the Trados one:
    ///
    ///   1. <c>GET {lcHost}/lc-api/gw-account-web/accounts/{tradosAccountId}</c> -> <c>businessAccountId</c>
    ///      and <c>productOffering</c>, the Trados licence that decides eligibility.
    ///   2. <c>GET {accountPortal}/account-portal/v1/weaver/details/{businessAccountId}</c> -> <see cref="LanguageWeaverDetails"/>
    ///   3. <c>GET {accountPortal}/account-portal/v1/weaver/trial</c>, with <c>X-Tenant: {businessAccountId}</c>
    ///      -> <see cref="TrialPeriod"/>. Trial branch only, and only for the dates: entitlement is already
    ///      settled by hop 2.
    ///
    /// Only Trados Go and Trados Freelance licences are eligible for Language Weaver Pro, so hop 1 also acts
    /// as a gate: an ineligible or unknown licence ends the check before any entitlement request is made.
    ///
    /// Every failure path returns <c>null</c>, which the decision service renders as "no pop-up". That is the
    /// specified behaviour for an undeterminable entitlement (DET-421, case E): show the user nothing, record
    /// the reason in the log, and re-evaluate on the next startup.
    /// </summary>
    public class CohereSubscriptionWorkflow : ICohereSubscriptionWorkflowService
    {
        private static string AccountPortalApiBaseUrl => CloudEnvironment.Current.AccountPortalApiBaseUrl;
        private const string AccountPortalDetailsPath = "account-portal/v1/weaver/details/";

        // Takes no path parameter: the account is named by the X-Tenant header instead.
        private const string AccountPortalTrialPath = "account-portal/v1/weaver/trial";
        private const string TenantHeader = "X-Tenant";

        // Documented host for this endpoint. Whether US-region accounts need a different one is still open with
        // the service team, so the request address is logged to make a wrong choice visible in the field.
        private static string LanguageCloudAccountsUrl => CloudEnvironment.Current.LanguageCloudAccountsUrl;

        private const string ProLanguagePairType = "GENERICPLUS";

        /// <summary>
        /// The Trados licences eligible for Language Weaver Pro. Confirmed against live accounts: a Go account
        /// reports <c>trados_go</c> and a Freelance one <c>trados_live_freelance</c> - note the inconsistent
        /// <c>live</c> segment, which is why these are recorded values rather than a derived pattern.
        /// </summary>
        private static readonly HashSet<string> EligibleProductOfferings =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "trados_go",
                "trados_live_freelance"
            };

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

            // The active tenant is only ever needed to look up the Account Portal record. It can legitimately be
            // empty for a valid session (no tenant selected yet), in which case there is nothing to evaluate:
            // both the entitlement and the licence are read from that record.
            if (string.IsNullOrWhiteSpace(languageCloudIdentity.ActiveTenantId))
            {
                Logger.Debug("[Cohere] No active tenant on this sign-in; entitlement is undeterminable.");
                return null;
            }

            var account = await GetAccountPortalIds(languageCloudIdentity.ActiveTenantId, authorizationHeaders);

            // Only Go and Freelance licences are eligible for Language Weaver Pro. Anything else is out of
            // scope for this prompt, so it is dropped before any entitlement request is made.
            if (!IsEligibleLicence(account.ProductOffering))
            {
                Logger.Info(
                    "[Cohere] Licence '{0}' is not eligible for Language Weaver Pro; no prompt will be shown.",
                    account.ProductOffering ?? "unknown");
                return null;
            }

            if (string.IsNullOrWhiteSpace(account.BusinessAccountId))
            {
                // Not a failure. The Account Portal record is created when a trial starts, so its absence
                // means this account has never started one - precisely the state the prompt exists to
                // address. There is no trial history to read, and no Pro to detect without it.
                Logger.Info("[Cohere] Eligible licence with no Account Portal record; treating as never trialed.");
                return MapEntitlement(hasProLanguagePairs: false);
            }

            var details = await GetLanguageWeaverDetails(account.BusinessAccountId, authorizationHeaders);
            if (details is null)
            {
                return null;
            }

            var data = MapDetails(details, account.BusinessAccountId, account.BusinessSubscriptionId);
            if (data is null)
            {
                // A deliberate cancellation: no prompt, and nothing to report beyond what MapDetails logged.
                return null;
            }

            // Only an account mid-trial has a countdown to show, so the extra request is confined to that case.
            if (data.IsTrial && !data.IsTrialExpired)
            {
                var trialPeriod = await GetTrialPeriod(account.BusinessAccountId, authorizationHeaders);
                data.TrialRemainingDays = trialPeriod?.RemainingDays();
            }

            Logger.Info(
                "[Cohere] Entitlement resolved: detected={0}, paid={1}, trial={2}, trialExpired={3}, admin={4}, trialDaysLeft={5}.",
                data.IsCohereDetected, data.IsPaid, data.IsTrial, data.IsTrialExpired, data.IsAdmin,
                data.TrialRemainingDays.HasValue ? data.TrialRemainingDays.Value.ToString() : "unknown");

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
        private static async Task<LanguageCloudAccount> GetAccountPortalIds(
            string tradosAccountId, Dictionary<string, string> headers)
        {
            var requestUri = LanguageCloudAccountsUrl + Uri.EscapeDataString(tradosAccountId);
            Logger.Debug("[Cohere] Resolving the Account Portal record via {0}", requestUri);

            var service = new GenericHTTPService<object, LanguageCloudAccountResponse>(
                HttpClient, headers, HttpMethod.Get, requestUri);
            var response = await service.SendRequest(null);

            if (!response.Success || response.Response?.Account is null)
            {
                Logger.Warn(
                    "[Cohere] Could not read the Language Cloud account from {0}: {1}",
                    requestUri, DescribeErrors(response?.Errors));

                // An empty record reads as an unknown licence, which the caller drops. That is the intended
                // outcome for an unreadable account: say nothing rather than guess at eligibility.
                return new LanguageCloudAccount();
            }

            return response.Response.Account;
        }

        /// <summary>
        /// Whether the account's Trados licence is eligible for Language Weaver Pro. An unknown or absent
        /// licence is not eligible: eligibility must be positively established, since the alternative is
        /// prompting users who cannot buy the add-on at all.
        /// </summary>
        private static bool IsEligibleLicence(string productOffering)
            => !string.IsNullOrWhiteSpace(productOffering)
            && EligibleProductOfferings.Contains(productOffering);

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
        /// Maps an account that has never started a trial. Both trial flags are false by knowledge rather than
        /// by ignorance: no Account Portal record exists, and that record is created when a trial starts.
        /// </summary>
        public static CohereSubscriptionData MapEntitlement(bool hasProLanguagePairs)
        {
            return new CohereSubscriptionData
            {
                IsCohereDetected = hasProLanguagePairs,
                IsPaid = hasProLanguagePairs,
                IsTrial = false,
                IsTrialExpired = false,
                IsAdmin = true
            };
        }

        /// <summary>
        /// Hop 3, on the trial branch only: reads the trial period so the prompt can say how many days are
        /// left. The account is named by the <c>X-Tenant</c> header - this endpoint takes no path parameter.
        /// <para>
        /// The header carries the Account Portal <c>businessAccountId</c>, the same identifier the sibling
        /// <c>/weaver/details/{businessAccountId}</c> endpoint is keyed on - not the Trados tenant id that
        /// hop 1 uses. Sending the Trados id here answers 403 <c>NOT_AUTHORIZED_EXCEPTION</c>, with a valid
        /// token and a live trial.
        /// </para>
        /// <para>
        /// Returns <c>null</c> on any failure. That is deliberately not fatal: the entitlement is already
        /// known from the details response by this point, and losing the countdown should cost the number in
        /// the copy, not the whole prompt.
        /// </para>
        /// </summary>
        private static async Task<TrialPeriod> GetTrialPeriod(string businessAccountId, Dictionary<string, string> headers)
        {
            var requestUri = new Uri(new Uri(AccountPortalApiBaseUrl), AccountPortalTrialPath).AbsoluteUri;

            var trialHeaders = new Dictionary<string, string>(headers)
            {
                [TenantHeader] = businessAccountId
            };

            var service = new GenericHTTPService<object, TrialPeriod>(
                HttpClient, trialHeaders, HttpMethod.Get, requestUri);
            var response = await service.SendRequest(null);

            if (!response.Success || response.Response is null)
            {
                Logger.Warn(
                    "[Cohere] Could not read the trial period from {0}: {1}",
                    requestUri, DescribeErrors(response?.Errors));
                return null;
            }

            return response.Response;
        }

        private static string DescribeErrors(IEnumerable<string> errors)
            => errors is null ? "no details" : string.Join("; ", errors);

        public static CohereSubscriptionData MapDetails(LanguageWeaverDetails details, string businessAccountId = null, string businessSubscriptionId = null)
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

            // CANCELLED means somebody deliberately cancelled - a trial or a paid subscription, and the
            // endpoint gives nothing to tell those apart. Either way the decision was intentional, so we do
            // not sell back to them: returning null renders no prompt at all (DET-421 case E).
            //
            // Note this must be an explicit early return rather than dropping CANCELLED from the terminal
            // set below: leaving it in place but out of IsCohereDetected would read as "never had Cohere"
            // and offer someone who just cancelled a 14-day free trial.
            if (!details.IsProActive
             && trialStatus.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase))
            {
                Logger.Info("[Cohere] Subscription was cancelled deliberately; showing no prompt.");
                return null;
            }

            // What is left is a trial that ran its course rather than one somebody stopped. ENDED is the only
            // such status: the service confirmed the full vocabulary is NOT_STARTED, IN_PROGRESS, CANCELLED
            // and ENDED.
            var hasEnded = trialStatus.Equals("ENDED", StringComparison.OrdinalIgnoreCase);

            return new CohereSubscriptionData
            {
                IsCohereDetected = details.IsProActive || isTrialRunning || hasEnded,
                // Converting to paid cancels the trial, so an active Pro add-on sits alongside a terminal
                // trial status. Pro therefore decides "paid" on its own: pairing it with the trial state
                // would show a paying customer the "trial ended" prompt.
                IsPaid = details.IsProActive,
                IsTrial = isTrialRunning || hasEnded,
                IsTrialExpired = hasEnded,
                // Every eligible account is treated as an administrator: the Account Portal record carries no
                // role, and the Language Weaver portal is no longer consulted for one.
                IsAdmin = true,
                BusinessAccountId = businessAccountId,
                BusinessSubscriptionId = businessSubscriptionId
            };
        }
    }
}

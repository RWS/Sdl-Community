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
    public class CohereSubscriptionWorkflow : ICohereSubscriptionWorkflowService
    {
        private static string AccountPortalApiBaseUrl => CloudEnvironment.Current.AccountPortalApiBaseUrl;
        private const string AccountPortalDetailsPath = "account-portal/v1/weaver/details/";
        private const string AccountPortalTrialPathIdentifiedByTenantHeader = "account-portal/v1/weaver/trial";

        // Must carry the Account Portal businessAccountId; the Trados tenant id answers 403 NOT_AUTHORIZED_EXCEPTION.
        private const string TenantHeader = "X-Tenant";

        // Open with the service team whether US-region accounts need a different host, hence the logged request address.
        private static string LanguageCloudAccountsUrl => CloudEnvironment.Current.LanguageCloudAccountsUrl;

        private const string ProLanguagePairType = "GENERICPLUS";

        // Service-confirmed vocabulary is NOT_STARTED, IN_PROGRESS, CANCELLED, ENDED - so ENDED is the only
        // terminal status nobody chose.
        private const string TrialInProgressStatus = "IN_PROGRESS";
        private const string TrialDeliberatelyCancelledStatus = "CANCELLED";
        private const string TrialRanItsCourseStatus = "ENDED";

        private const bool EveryEligibleAccountIsTreatedAsAdministrator = true;

        // Recorded from live accounts, not a derivable pattern - note the inconsistent "live" segment.
        private static readonly HashSet<string> EligibleProductOfferings =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "trados_go",
                "trados_live_freelance"
            };

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly HttpClient HttpClient = new HttpClient();

        private readonly string _trialStatusOverride;

        public CohereSubscriptionWorkflow()
            : this(null)
        {
        }

        public CohereSubscriptionWorkflow(string trialStatusOverride)
        {
            _trialStatusOverride = trialStatusOverride;
        }

        private void ApplyTheDeveloperTrialStatusOverride(LanguageWeaverDetails details)
        {
            if (string.IsNullOrWhiteSpace(_trialStatusOverride))
                return;

            Logger.Warn(
                "[Cohere] Developer settings replace the reported trialStatus '{0}' with '{1}'.",
                details.TrialStatus ?? "none", _trialStatusOverride);

            details.TrialStatus = _trialStatusOverride;
        }

        public async Task<CohereSubscriptionData> ExecuteAsync()
        {
            var languageCloudIdentity = LanguageCloudIdentityApi.Instance;
            if (languageCloudIdentity is null
             || string.IsNullOrWhiteSpace(languageCloudIdentity.AccessToken))
            {
                Logger.Debug("[Cohere] No Language Cloud session (no access token); skipping entitlement check.");
                return null;
            }

            var authorizationHeaders = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {languageCloudIdentity.AccessToken}"
            };

            if (string.IsNullOrWhiteSpace(languageCloudIdentity.ActiveTenantId))
            {
                Logger.Debug("[Cohere] No active tenant on this sign-in; entitlement is undeterminable.");
                return null;
            }

            var account = await GetAccountPortalIds(languageCloudIdentity.ActiveTenantId, authorizationHeaders);

            if (!IsEligibleLicence(account.ProductOffering))
            {
                Logger.Info(
                    "[Cohere] Licence '{0}' is not eligible for Language Weaver Pro; no prompt will be shown.",
                    account.ProductOffering ?? "unknown");
                return null;
            }

            if (string.IsNullOrWhiteSpace(account.BusinessAccountId))
            {
                // The record is created when a trial starts, so its absence means one never was.
                Logger.Info("[Cohere] Eligible licence with no Account Portal record; treating as never trialed.");
                return MapEntitlement(hasProLanguagePairs: false);
            }

            var details = await GetLanguageWeaverDetails(account.BusinessAccountId, authorizationHeaders);
            if (details is null)
            {
                return null;
            }

            ApplyTheDeveloperTrialStatusOverride(details);

            var data = MapDetails(details, account.BusinessAccountId, account.BusinessSubscriptionId);
            if (data is null)
            {
                return null;
            }

            if (HasACountdownToShow(data))
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

                return AnAccountWithAnUnknownLicence();
            }

            return response.Response.Account;
        }

        private static LanguageCloudAccount AnAccountWithAnUnknownLicence() => new LanguageCloudAccount();

        private static bool IsEligibleLicence(string productOffering)
            => !string.IsNullOrWhiteSpace(productOffering)
            && EligibleProductOfferings.Contains(productOffering);

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

        public static CohereSubscriptionData MapEntitlement(bool hasProLanguagePairs)
        {
            return new CohereSubscriptionData
            {
                IsCohereDetected = hasProLanguagePairs,
                IsPaid = hasProLanguagePairs,
                IsTrial = false,
                IsTrialExpired = false,
                IsAdmin = EveryEligibleAccountIsTreatedAsAdministrator
            };
        }

        private static async Task<TrialPeriod> GetTrialPeriod(string businessAccountId, Dictionary<string, string> headers)
        {
            var requestUri = new Uri(new Uri(AccountPortalApiBaseUrl), AccountPortalTrialPathIdentifiedByTenantHeader).AbsoluteUri;

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

            var trialStatus = (details.TrialStatus ?? string.Empty).Trim();
            var isTrialRunning = Is(trialStatus, TrialInProgressStatus);
            var hasTrialRunItsCourse = Is(trialStatus, TrialRanItsCourseStatus);

            if (WasDeliberatelyCancelled(details, trialStatus))
            {
                Logger.Info("[Cohere] Subscription was cancelled deliberately; showing no prompt.");
                return null;
            }

            return new CohereSubscriptionData
            {
                IsCohereDetected = details.IsProActive || isTrialRunning || hasTrialRunItsCourse,
                IsPaid = details.IsProActive,
                IsTrial = isTrialRunning || hasTrialRunItsCourse,
                IsTrialExpired = hasTrialRunItsCourse,
                IsAdmin = EveryEligibleAccountIsTreatedAsAdministrator,
                BusinessAccountId = businessAccountId,
                BusinessSubscriptionId = businessSubscriptionId
            };
        }

        private static bool HasACountdownToShow(CohereSubscriptionData data)
            => data.IsTrial && !data.IsTrialExpired;

        private static bool WasDeliberatelyCancelled(LanguageWeaverDetails details, string trialStatus)
            => !details.IsProActive && Is(trialStatus, TrialDeliberatelyCancelledStatus);

        private static bool Is(string trialStatus, string status)
            => trialStatus.Equals(status, StringComparison.OrdinalIgnoreCase);
    }
}

using LanguageWeaverProvider.CohereSubscription.Workflow.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using LanguageWeaverProvider.Infrastructure.Http.Services;
using Sdl.LanguageCloud.IdentityApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.CohereSubscription.Workflow.Services
{
    public class CohereSubscriptionWorkflow : ICohereSubscriptionWorkflowService
    {
        private const string AccountPortalDetailsPath = "account-portal/v1/weaver/details/";
        private const string AccountPortalApiBaseUrl = "https://account-portal-api.sdl.com/";

        public async Task<CohereSubscriptionData> ExecuteAsync()
        {
            var languageCloudIdentity = LanguageCloudIdentityApi.Instance;
            if (languageCloudIdentity is null ||
                string.IsNullOrWhiteSpace(languageCloudIdentity.AccessToken) ||
                string.IsNullOrWhiteSpace(languageCloudIdentity.ActiveTenantId))
            {
                return null;
            }

            var recurlyAccountId = Uri.EscapeDataString(languageCloudIdentity.ActiveTenantId);
            var requestUri = new Uri(new Uri(AccountPortalApiBaseUrl), AccountPortalDetailsPath + recurlyAccountId).AbsoluteUri;
            var headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {languageCloudIdentity.AccessToken}"
            };
            var service = new GenericHTTPService<object, LanguageWeaverDetails>(
                new HttpClient(), headers, HttpMethod.Get, requestUri);
            var response = await service.SendRequest(null);

            return response.Success && response.Response is not null
                ? MapDetails(response.Response)
                : null;
        }

        public static CohereSubscriptionData MapDetails(LanguageWeaverDetails details)
        {
            if (details is null)
            {
                return null;
            }

            var trialStatus = details.TrialStatus ?? string.Empty;
            var isTrial = trialStatus.IndexOf("trial", StringComparison.OrdinalIgnoreCase) >= 0;
            var isTrialExpired = isTrial &&
                (trialStatus.IndexOf("expired", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 trialStatus.IndexOf("ended", StringComparison.OrdinalIgnoreCase) >= 0);

            return new CohereSubscriptionData
            {
                IsCohereDetected = details.IsProActive || isTrial,
                IsPaid = details.IsProActive && !isTrial,
                IsTrial = isTrial,
                IsTrialExpired = isTrialExpired,
                // The endpoint does not include role information. Treat the user as non-admin so the prompt
                // never presents subscription actions that the current user may not be authorized to perform.
                IsAdmin = false
            };
        }
    }
}

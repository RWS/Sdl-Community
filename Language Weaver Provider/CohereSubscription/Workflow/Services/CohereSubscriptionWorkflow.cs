using LanguageWeaverProvider.CohereSubscription.Workflow.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using LanguageWeaverProvider.Infrastructure.Http.Model;
using LanguageWeaverProvider.Infrastructure.Http.Services;
using LanguageWeaverProvider.Model;
using Sdl.LanguageCloud.IdentityApi;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Navigation;

namespace LanguageWeaverProvider.CohereSubscription.Workflow.Services
{
    public class CohereSubscriptionWorkflow : ICohereSubscriptionWorkflowService
    {
        public async Task<CohereSubscriptionData> ExecuteAsync()
        {
            var lcInstance = LanguageCloudIdentityApi.Instance;
            if (lcInstance is null)
            {
                // Log
                return null;
            }

            var languageCloudToken = $"Bearer {lcInstance.AccessToken}";
            var tenantId = lcInstance.ActiveTenantId;

            // TODO
            // Create the genericHttpService with the instances that we'll use 
            // When the final request is done, return the CohereSubscriptionData
            // And add logic for the URIs
            var cohereNotDetectedAdmin = new CohereSubscriptionData
            {
                IsCohereDetected = false,
                IsPaid = false,
                IsTrial = false,
                IsAdmin = true,
            };

            var cohereNotDetectedNotAdmin = new CohereSubscriptionData
            {
                IsCohereDetected = false,
                IsPaid = false,
                IsTrial = false,
                IsAdmin = false,
            };


            var trialActiveAdmin = new CohereSubscriptionData()
            {
                IsCohereDetected = true,
                IsTrial = true,
                IsAdmin = true,
                TrialRemainingDays = 3
            };

            var trialActiveNotAdmin = new CohereSubscriptionData()
            {
                IsCohereDetected = true,
                IsTrial = true,
                IsAdmin = false,
                TrialRemainingDays = 3
            };

            var trialExpiredAdmin = new CohereSubscriptionData
            {
                IsCohereDetected = true,
                IsTrialExpired = true,
                IsAdmin = true,
            };

            var trialExpiredNotAdmin = new CohereSubscriptionData
            {
                IsCohereDetected = true,
                IsTrial = true,
                IsTrialExpired = true,
                IsAdmin = false,
            };

            return cohereNotDetectedAdmin;
            //return cohereNotDetectedNotAdmin;
            //return trialActiveAdmin;
            //return trialActiveNotAdmin
            //return trialExpiredAdmin;
            //return trialExpiredNotAdmin;
        }
    }
}

using LanguageWeaverProvider.CohereSubscription.Workflow.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.CohereSubscription.Workflow.Services
{
    public class CohereSubscriptionWorkflow : ICohereSubscriptionWorkflowService
    {
        public Task<CohereSubscriptionData> ExecuteAsync()
        {
            var fakeData = new CohereSubscriptionData
            {
                IsCohereDetected = false,
                IsPaid = false,
                IsTrial = true,
                IsTrialExpired = false,
                IsAdmin = false,
                TrialRemainingDays = 3 // example
            };

            return Task.FromResult(fakeData);
        }
    }
}

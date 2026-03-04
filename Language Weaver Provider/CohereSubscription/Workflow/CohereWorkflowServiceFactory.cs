using LanguageWeaverProvider.CohereSubscription.Workflow.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Workflow.Services;

namespace LanguageWeaverProvider.CohereSubscription.Workflow
{
    public class CohereWorkflowServiceFactory
    {
        public static ICohereSubscriptionWorkflowService Create()
        {
            return new CohereSubscriptionWorkflow();
        }
    }
}

using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using LanguageWeaverProvider.SubscriptionJourney.ViewModel;

namespace LanguageWeaverProvider.CohereSubscription.Decision.Interfaces
{
    public interface ICohereSubscriptionDecisionService
    {
        SubscriptionViewModel BuildViewModel(CohereSubscriptionData data);
    }
}

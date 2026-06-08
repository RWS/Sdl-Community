using LanguageWeaverProvider.CohereSubscription.Decision.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Decision.Services;
using System;

namespace LanguageWeaverProvider.CohereSubscription.Decision
{
    public class CohereSubscriptionDecisionServiceFactory
    {
        public static ICohereSubscriptionDecisionService Create()
        {
            return new CohereSubscriptionDecisionService();
        }
    }
}

using LanguageWeaverProvider.CohereSubscription.Decision.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using LanguageWeaverProvider.SubscriptionJourney.Model;
using LanguageWeaverProvider.SubscriptionJourney.Services;
using LanguageWeaverProvider.SubscriptionJourney.ViewModel;

namespace LanguageWeaverProvider.CohereSubscription.Decision.Services
{
    public class CohereSubscriptionDecisionService : ICohereSubscriptionDecisionService
    {
        public SubscriptionViewModel BuildViewModel(CohereSubscriptionData data)
        {
            if (data == null)
                return null;

            // Paid add-on: no pop-up
            if (data.IsPaid)
                return null;

            // Cohere not detected
            if (!data.IsCohereDetected)
            {
                var options = BuildOptions(
                    title: "Cohere Subscription",
                    description: "Try Cohere for 14 days free and see the benefits.",
                    isAdmin: data.IsAdmin,
                    trialDays: 14
                );

                return new SubscriptionViewModel("Cohere Subscription", options, new UriOpener());
            }

            // Trial active: only show if 7–1 days remaining
            if (data.IsTrial && !data.IsTrialExpired)
            {
                if (data.TrialRemainingDays > 7)
                    return null;

                var options = BuildOptions(
                    title: "Cohere Trial",
                    description: $"Your trial expires in {data.TrialRemainingDays} day(s).",
                    isAdmin: data.IsAdmin,
                    trialDays: data.TrialRemainingDays
                );

                return new SubscriptionViewModel("Cohere Trial", options, new UriOpener());
            }

            // Trial expired
            if (data.IsTrial && data.IsTrialExpired)
            {
                var options = BuildOptions(
                    title: "Cohere Trial Expired",
                    description: "Your trial has expired.",
                    isAdmin: data.IsAdmin
                );

                return new SubscriptionViewModel(
                    "Cohere Trial Expired", options, new UriOpener());
            }

            return null;
        }

        private SubscriptionOptions BuildOptions(string title, string description, bool isAdmin, int trialDays = 0)
        {
            if (isAdmin)
            {
                return new SubscriptionOptions
                {
                    Title = title,
                    Description = description,
                    IsDoNotShowAgainVisible = true,
                    DoNotShowThisAgain = false,
                    ShowPrimary = true,
                    PrimaryContent = trialDays > 0 ? "Start Free Trial" : "Buy Now",
                    TrialUri = "https://account.portal/trial",
                    ShowSecondary = trialDays > 0 ? true : false,
                    SecondaryContent = "Buy Now",
                    BuyUri = "https://account.portal/buy",
                    CancelContent = "Cancel"
                };
            }
            else
            {
                return new SubscriptionOptions
                {
                    Title = title,
                    Description = description,
                    IsDoNotShowAgainVisible = true,
                    DoNotShowThisAgain = false,
                    ShowPrimary = true,
                    PrimaryContent = trialDays > 0 ? "Contact your Admin" : "Contact Admin to purchase/extend access",
                    ShowSecondary = true,
                    SecondaryContent = "Learn more",
                    CancelContent = "Cancel"
                };
            }
        }
    }
}

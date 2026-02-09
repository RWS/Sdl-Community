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

            var uriOpener = new UriOpener();
            // Cohere not detected
            if (!data.IsCohereDetected)
            {
                if (data.IsAdmin)
                {
                    return new SubscriptionViewModel(
                        "Cohere Subscription",
                        new SubscriptionOptions()
                        {
                            Title = "Try the best translation LLM in the world!",
                            Description = "Unleash unprecedented translation power with the Trados LLM, powered by Cohere and Language Weaver. Start a 14‑day free trial or buy the add-on anytime.",
                            ShowPrimary = true,
                            PrimaryContent = "Start free trial",
                            BuyUri = "https://example.com/",
                            ShowSecondary = true,
                            SecondaryContent = "Buy now",
                            TrialUri = "https://example.com/",
                            CancelContent = "Cancel",
                            IsDoNotShowAgainVisible = true,
                        },
                        uriOpener);

                }

                return new SubscriptionViewModel(
                    "Cohere Subscription",
                    new SubscriptionOptions
                    {
                        Title = "Trados LLM available for your organization",
                        Description = "The brand new Trados LLM (powered by Cohere) can be enabled on your Language Weaver account. Ask your administrator to start a 14‑day free trial or purchase the add-on.",
                        ShowPrimary = true,
                        PrimaryContent = "OK",
                        ShowSecondary = true,
                        SecondaryContent = "Learn more",
                        TrialUri = "https://example.com/",
                        CancelContent = "Cancel",
                        IsDoNotShowAgainVisible = true,
                    },
                    uriOpener);
            }

            // Trial active: only show if 7–1 days remaining
            if (data.IsTrial && !data.IsTrialExpired)
            {
                if (data.TrialRemainingDays > 7)
                    return null;

                if (data.IsAdmin)
                {
                    return new SubscriptionViewModel(
                        "Cohere Subscription",
                        new SubscriptionOptions
                        {
                            Title = "Trados LLM trial ending soon",
                            Description = $"Your Trados LLM (powered by Cohere) trial ends in {data.TrialRemainingDays} day(s). Purchase the add-on to keep using the LLM without interruption.",
                            ShowPrimary = true,
                            PrimaryContent = "Buy now",
                            BuyUri = "https://example.com/",
                            ShowSecondary = false,
                            CancelContent = "Cancel",
                            IsDoNotShowAgainVisible = true,
                        },
                        uriOpener);

                }

                return new SubscriptionViewModel(
                  "Cohere Subscription",
                  new SubscriptionOptions
                  {
                      Title = "Trados LLM trial ending soon",
                      Description = $"Your organization’s Trados LLM trial ends in {data.TrialRemainingDays} day(s). Contact your administrator to purchase the add-on and avoid interruption.",
                      ShowPrimary = true,
                      PrimaryContent = "OK",
                      ShowSecondary = true,
                      SecondaryContent = "Learn more",
                      TrialUri = "https://example.com/",
                      CancelContent = "Cancel",
                      IsDoNotShowAgainVisible = true,
                  },
                  uriOpener);
            }

            // Trial expired
            if (data.IsTrial && data.IsTrialExpired)
            {
                if (data.IsAdmin)
                {
                    return new SubscriptionViewModel(
                        "Cohere Subscription",
                        new SubscriptionOptions
                        {
                            Title = "Trados LLM trial expired",
                            Description = "Your 14‑day Trados LLM trial has ended. Purchase the add-on to continue using the LLM.",
                            ShowPrimary = true,
                            PrimaryContent = "Buy now",
                            BuyUri = "https://example.com/",
                            ShowSecondary = false,
                            CancelContent = "Cancel",
                            IsDoNotShowAgainVisible = true,
                        }, uriOpener);
                }

                return new SubscriptionViewModel(
                    "Cohere Subscription",
                    new SubscriptionOptions
                    {
                        Title = "Trados LLM trial expired",
                        Description = "Your organization’s Trados LLM trial has ended. Contact your administrator to purchase the add-on to restore access.",
                        ShowPrimary = true,
                        PrimaryContent = "OK",
                        ShowSecondary = true,
                        SecondaryContent = "Learn more",
                        TrialUri = "https://example.com/",
                        CancelContent = "Cancel",
                        IsDoNotShowAgainVisible = true,
                    }, uriOpener);
            }

            return null;
        }
    }
}

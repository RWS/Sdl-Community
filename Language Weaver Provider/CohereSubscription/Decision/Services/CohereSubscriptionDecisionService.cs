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
                            Description = "Unleash unprecedented translation power with the Trados LLM, powered by Cohere and Language Weaver. \n\nStart a 14‑day free trial or buy the add-on anytime.",
                            ShowPrimary = true,
                            PrimaryContent = "Start free trial",
                            ShowSecondary = true,
                            SecondaryContent = "Buy now",
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
                        Description = "The brand new Trados LLM (powered by Cohere) can be enabled on your Language Weaver account. \n\nAsk your administrator to start a 14‑day free trial or purchase the add-on.",
                        ShowPrimary = true,
                        PrimaryContent = "OK",
                        ShowSecondary = true,
                        SecondaryContent = "Learn more",
                        CancelContent = "Cancel",
                        IsDoNotShowAgainVisible = true,
                    },
                    uriOpener);
            }

            // The account-portal response identifies an active trial but does not provide an end date.
            if (data.IsTrial && !data.IsTrialExpired)
            {
                if (data.IsAdmin)
                {
                    return new SubscriptionViewModel(
                        "Cohere Subscription",
                        new SubscriptionOptions
                        {
                            Title = "Trados LLM trial active",
                            Description = "Your Trados LLM (powered by Cohere) trial is active. \n\nPurchase the add-on to keep using the LLM without interruption when the trial ends.",
                            ShowPrimary = true,
                            PrimaryContent = "Buy now",
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
                      Title = "Trados LLM trial active",
                      Description = "Your organization’s Trados LLM trial is active. \n\nContact your administrator to purchase the add-on before the trial ends.",
                      ShowPrimary = true,
                      PrimaryContent = "OK",
                      ShowSecondary = true,
                      SecondaryContent = "Learn more",
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
                            Description = "Your 14‑day Trados LLM trial has ended. \n\nPurchase the add-on to continue using the LLM.",
                            ShowPrimary = true,
                            PrimaryContent = "Buy now",
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
                        Description = "Your organization’s Trados LLM trial has ended. \n\nContact your administrator to purchase the add-on to restore access.",
                        ShowPrimary = true,
                        PrimaryContent = "OK",
                        ShowSecondary = true,
                        SecondaryContent = "Learn more",
                        CancelContent = "Cancel",
                        IsDoNotShowAgainVisible = true,
                    }, uriOpener);
            }

            return null;
        }
    }
}

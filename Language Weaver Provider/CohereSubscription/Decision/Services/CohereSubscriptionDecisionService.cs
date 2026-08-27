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

            // An account provisioned through Account Portal is administered on its own tenant page there;
            // one without such a record has nothing to link to, so it goes to the Language Weaver portal
            // account page instead. Both actions land on the same dashboard: buying and starting a trial are
            // both done from there, and neither can be deep-linked from outside the portal.
            var accountUri = string.IsNullOrWhiteSpace(data.BusinessAccountId)
                ? Constants.LanguageWeaverProStartTrialUrl
                : Constants.AccountPortalTenantUrl + data.BusinessAccountId;

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
                            PrimaryUri = accountUri,
                            ShowSecondary = true,
                            SecondaryContent = "Buy now",
                            SecondaryUri = accountUri,
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
                        SecondaryUri = Constants.LanguageWeaverProLearnMoreUrl,
                        CancelContent = "Cancel",
                        IsDoNotShowAgainVisible = true,
                    },
                    uriOpener);
            }

            // Unreachable while nothing exposes trial state; the workflow leaves both trial flags false.
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
                            PrimaryUri = accountUri,
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
                      SecondaryUri = Constants.LanguageWeaverProLearnMoreUrl,
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
                            PrimaryUri = accountUri,
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
                        SecondaryUri = Constants.LanguageWeaverProLearnMoreUrl,
                        CancelContent = "Cancel",
                        IsDoNotShowAgainVisible = true,
                    }, uriOpener);
            }

            return null;
        }
    }
}

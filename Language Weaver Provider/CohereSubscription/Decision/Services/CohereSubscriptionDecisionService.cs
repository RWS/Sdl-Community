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
                            Title = "Try Language Weaver Pro free for 14 days",
                            Description = "Language Weaver Pro is RWS's most advanced AI translation solution, built in partnership with Cohere. It ranked first in 31 of 32 languages in benchmarking tests. \n\nStart a 14-day free trial, or add it to your subscription at any time.",
                            ShowPrimary = true,
                            PrimaryContent = "Start free trial",
                            PrimaryUri = Constants.LanguageWeaverProPricingUrl,
                            ShowSecondary = true,
                            SecondaryContent = "Buy now",
                            SecondaryUri = Constants.LanguageWeaverProPricingUrl,
                            CancelContent = "Cancel",
                            IsDoNotShowAgainVisible = true,
                        },
                        uriOpener);

                }

                return new SubscriptionViewModel(
                    "Cohere Subscription",
                    new SubscriptionOptions
                    {
                        Title = "Language Weaver Pro is available for your organization",
                        Description = "Language Weaver Pro is RWS's most advanced AI translation solution, built in partnership with Cohere, and can be enabled on your Language Weaver account. \n\nAsk your administrator to start a 14-day free trial or add it to your subscription.",
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
                            Title = "Your Language Weaver Pro trial is active",
                            Description = "Add Language Weaver Pro to your subscription to keep using it without interruption when the trial ends.",
                            ShowPrimary = true,
                            PrimaryContent = "Buy now",
                            PrimaryUri = Constants.LanguageWeaverProPricingUrl,
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
                      Title = "Your Language Weaver Pro trial is active",
                      Description = "Your organization’s Language Weaver Pro trial is active. \n\nContact your administrator to add it to your subscription before the trial ends.",
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
                            Title = "Your Language Weaver Pro trial has ended",
                            Description = "Your 14-day Language Weaver Pro trial has ended. \n\nAdd it to your subscription to continue using it.",
                            ShowPrimary = true,
                            PrimaryContent = "Buy now",
                            PrimaryUri = Constants.LanguageWeaverProPricingUrl,
                            ShowSecondary = false,
                            CancelContent = "Cancel",
                            IsDoNotShowAgainVisible = true,
                        }, uriOpener);
                }

                return new SubscriptionViewModel(
                    "Cohere Subscription",
                    new SubscriptionOptions
                    {
                        Title = "Your Language Weaver Pro trial has ended",
                        Description = "Your organization’s Language Weaver Pro trial has ended. \n\nContact your administrator to add it to your subscription to restore access.",
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

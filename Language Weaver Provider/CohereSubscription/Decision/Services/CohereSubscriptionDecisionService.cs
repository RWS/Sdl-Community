using LanguageWeaverProvider.CohereSubscription.Decision.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.SubscriptionJourney.Model;
using LanguageWeaverProvider.SubscriptionJourney.Services;
using LanguageWeaverProvider.SubscriptionJourney.ViewModel;

namespace LanguageWeaverProvider.CohereSubscription.Decision.Services
{
    public class CohereSubscriptionDecisionService : ICohereSubscriptionDecisionService
    {
        public const int DefaultTrialPromptFromDaysRemaining = 7;

        private readonly int _trialPromptFromDaysRemaining;

        public CohereSubscriptionDecisionService()
            : this(DefaultTrialPromptFromDaysRemaining)
        {
        }

        public CohereSubscriptionDecisionService(int trialPromptFromDaysRemaining)
        {
            _trialPromptFromDaysRemaining = trialPromptFromDaysRemaining;
        }

        public SubscriptionViewModel BuildViewModel(CohereSubscriptionData data)
        {
            if (data == null)
                return null;

            if (data.IsPaid)
                return null;

            var uriOpener = new UriOpener();

            var accountUri = AccountUriFor(data);

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

            if (IsMidTrial(data))
            {
                if (IsStillOutsideTheCountdownWindow(data))
                {
                    return null;
                }

                return BuildActiveTrialViewModel(data, accountUri, uriOpener);
            }

            if (HasTrialRunItsCourse(data))
            {
                if (data.IsAdmin)
                {
                    return new SubscriptionViewModel(
                        "Cohere Subscription",
                        new SubscriptionOptions
                        {
                            Title = "Trados LLM trial ended",
                            Description = "Your Trados LLM (powered by Cohere) trial has ended. \n\nPurchase the add-on to continue using the LLM.",
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
                        Title = "Trados LLM trial ended",
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

        private static string AccountUriFor(CohereSubscriptionData data)
            => string.IsNullOrWhiteSpace(data.BusinessAccountId)
                ? Constants.LanguageWeaverProStartTrialUrl
                : Constants.AccountPortalTenantUrl + data.BusinessAccountId;

        private static bool IsMidTrial(CohereSubscriptionData data)
            => data.IsTrial && !data.IsTrialExpired;

        private static bool HasTrialRunItsCourse(CohereSubscriptionData data)
            => data.IsTrial && data.IsTrialExpired;

        private bool IsStillOutsideTheCountdownWindow(CohereSubscriptionData data)
            => !data.TrialRemainingDays.HasValue
            || data.TrialRemainingDays.Value > _trialPromptFromDaysRemaining;

        private static SubscriptionViewModel BuildActiveTrialViewModel(CohereSubscriptionData data, string accountUri, IUriOpener uriOpener)
        {
            var daysRemaining = data.TrialRemainingDays ?? 0;
            var endsIn = daysRemaining == 0
                ? "ends today"
                : daysRemaining == 1
                    ? "ends in 1 day"
                    : $"ends in {daysRemaining} days";

            if (data.IsAdmin)
            {
                return new SubscriptionViewModel(
                    "Cohere Subscription",
                    new SubscriptionOptions
                    {
                        Title = "Trados LLM trial active",
                        Description = $"Your Trados LLM (powered by Cohere) trial {endsIn}. \n\nPurchase the add-on to keep using the LLM without interruption when the trial ends.",
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
                    Description = $"Your organization’s Trados LLM trial {endsIn}. \n\nContact your administrator to purchase the add-on before the trial ends.",
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
    }
}

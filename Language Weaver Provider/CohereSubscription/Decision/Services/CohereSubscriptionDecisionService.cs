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
                            Title = "Try the best AI translation in the world!",
                            Description = "Language Weaver Pro gives you the highest quality AI translation, for a stronger first pass draft. \n\nStart your 14-day free trial or buy the add-on anytime.",
                            ShowPrimary = true,
                            PrimaryContent = "Start free trial",
                            PrimaryUri = accountUri,
                            ShowSecondary = true,
                            SecondaryContent = "Buy now",
                            SecondaryUri = accountUri,
                            LearnMoreUri = Constants.LanguageWeaverProLearnMoreUrl,
                            CancelContent = "Cancel",
                            IsDoNotShowAgainVisible = true,
                        },
                        uriOpener);

                }

                return new SubscriptionViewModel(
                    "Cohere Subscription",
                    new SubscriptionOptions
                    {
                        Title = "Language Weaver Pro available for your organization",
                        Description = "The world’s best AI translation engine, Language Weaver Pro, can be enabled on your Language Weaver account. \n\nAsk your administrator to start a 14-day free trial or purchase the add-on.",
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
                            Title = "Language Weaver Pro trial expired",
                            Description = "Your 14-day Language Weaver Pro trial has ended. \n\nPurchase the add-on to continue using the world’s best AI translation.",
                            ShowPrimary = true,
                            PrimaryContent = "Buy now",
                            PrimaryUri = accountUri,
                            ShowSecondary = false,
                            LearnMoreUri = Constants.LanguageWeaverProLearnMoreUrl,
                            CancelContent = "Cancel",
                            IsDoNotShowAgainVisible = true,
                        }, uriOpener);
                }

                return new SubscriptionViewModel(
                    "Cohere Subscription",
                    new SubscriptionOptions
                    {
                        Title = "Language Weaver Pro trial expired",
                        Description = "Your organization’s Language Weaver Pro trial has ended. \n\nContact your administrator to purchase the add-on to restore access.",
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
                ? Constants.AccountPortalUrl
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
                        Title = "Language Weaver Pro trial ending soon",
                        Description = $"Your Language Weaver Pro trial {endsIn}. \n\nPurchase the add-on to keep using the world’s best AI translation without interruption.",
                        ShowPrimary = true,
                        PrimaryContent = "Buy now",
                        PrimaryUri = accountUri,
                        ShowSecondary = false,
                        LearnMoreUri = Constants.LanguageWeaverProLearnMoreUrl,
                        CancelContent = "Cancel",
                        IsDoNotShowAgainVisible = true,
                    },
                    uriOpener);
            }

            return new SubscriptionViewModel(
                "Cohere Subscription",
                new SubscriptionOptions
                {
                    Title = "Language Weaver Pro trial ending soon",
                    Description = $"Your organization’s Language Weaver Pro trial {endsIn}. \n\nContact your administrator to purchase the add-on and avoid any interruption.",
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

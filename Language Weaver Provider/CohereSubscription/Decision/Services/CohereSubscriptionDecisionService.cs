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
                        PluginResources.Cohere_WindowTitle,
                        new SubscriptionOptions()
                        {
                            Title = PluginResources.Cohere_NotDetected_Admin_Title,
                            Description = PluginResources.Cohere_NotDetected_Admin_Body,
                            ShowPrimary = true,
                            PrimaryContent = PluginResources.Cohere_Button_StartFreeTrial,
                            PrimaryUri = accountUri,
                            ShowSecondary = true,
                            SecondaryContent = PluginResources.Cohere_Button_BuyNow,
                            SecondaryUri = accountUri,
                            LearnMoreUri = Constants.LanguageWeaverProLearnMoreUrl,
                            CancelContent = PluginResources.Cohere_Button_Cancel,
                            IsDoNotShowAgainVisible = true,
                        },
                        uriOpener);

                }

                return new SubscriptionViewModel(
                    PluginResources.Cohere_WindowTitle,
                    new SubscriptionOptions
                    {
                        Title = PluginResources.Cohere_NotDetected_NonAdmin_Title,
                        Description = PluginResources.Cohere_NotDetected_NonAdmin_Body,
                        ShowPrimary = true,
                        PrimaryContent = PluginResources.Cohere_Button_Ok,
                        ShowSecondary = true,
                        SecondaryContent = PluginResources.Cohere_Button_LearnMore,
                        SecondaryUri = Constants.LanguageWeaverProLearnMoreUrl,
                        CancelContent = PluginResources.Cohere_Button_Cancel,
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
                        PluginResources.Cohere_WindowTitle,
                        new SubscriptionOptions
                        {
                            Title = PluginResources.Cohere_TrialExpired_Admin_Title,
                            Description = PluginResources.Cohere_TrialExpired_Admin_Body,
                            ShowPrimary = true,
                            PrimaryContent = PluginResources.Cohere_Button_BuyNow,
                            PrimaryUri = accountUri,
                            ShowSecondary = false,
                            LearnMoreUri = Constants.LanguageWeaverProLearnMoreUrl,
                            CancelContent = PluginResources.Cohere_Button_Cancel,
                            IsDoNotShowAgainVisible = true,
                        }, uriOpener);
                }

                return new SubscriptionViewModel(
                    PluginResources.Cohere_WindowTitle,
                    new SubscriptionOptions
                    {
                        Title = PluginResources.Cohere_TrialExpired_NonAdmin_Title,
                        Description = PluginResources.Cohere_TrialExpired_NonAdmin_Body,
                        ShowPrimary = true,
                        PrimaryContent = PluginResources.Cohere_Button_Ok,
                        ShowSecondary = true,
                        SecondaryContent = PluginResources.Cohere_Button_LearnMore,
                        SecondaryUri = Constants.LanguageWeaverProLearnMoreUrl,
                        CancelContent = PluginResources.Cohere_Button_Cancel,
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
                ? PluginResources.Cohere_Trial_EndsToday
                : daysRemaining == 1
                    ? PluginResources.Cohere_Trial_EndsInOneDay
                    : string.Format(PluginResources.Cohere_Trial_EndsInDays, daysRemaining);

            if (data.IsAdmin)
            {
                return new SubscriptionViewModel(
                    PluginResources.Cohere_WindowTitle,
                    new SubscriptionOptions
                    {
                        Title = PluginResources.Cohere_TrialEnding_Admin_Title,
                        Description = string.Format(PluginResources.Cohere_TrialEnding_Admin_Body, endsIn),
                        ShowPrimary = true,
                        PrimaryContent = PluginResources.Cohere_Button_BuyNow,
                        PrimaryUri = accountUri,
                        ShowSecondary = false,
                        LearnMoreUri = Constants.LanguageWeaverProLearnMoreUrl,
                        CancelContent = PluginResources.Cohere_Button_Cancel,
                        IsDoNotShowAgainVisible = true,
                    },
                    uriOpener);
            }

            return new SubscriptionViewModel(
                PluginResources.Cohere_WindowTitle,
                new SubscriptionOptions
                {
                    Title = PluginResources.Cohere_TrialEnding_NonAdmin_Title,
                    Description = string.Format(PluginResources.Cohere_TrialEnding_NonAdmin_Body, endsIn),
                    ShowPrimary = true,
                    PrimaryContent = PluginResources.Cohere_Button_Ok,
                    ShowSecondary = true,
                    SecondaryContent = PluginResources.Cohere_Button_LearnMore,
                    SecondaryUri = Constants.LanguageWeaverProLearnMoreUrl,
                    CancelContent = PluginResources.Cohere_Button_Cancel,
                    IsDoNotShowAgainVisible = true,
                },
                uriOpener);
        }
    }
}

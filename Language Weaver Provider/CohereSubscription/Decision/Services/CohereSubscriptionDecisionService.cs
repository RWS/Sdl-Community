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
        /// <summary>
        /// DET-421: silence on days 14-8, then a countdown from day 7. This is the shipped rule; the
        /// developer settings file may override it so the countdown can be exercised without a rebuild.
        /// </summary>
        public const int DefaultTrialPromptFromDaysRemaining = 7;

        /// <summary>
        /// The active-trial prompt appears once the trial has this many days left or fewer.
        /// </summary>
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

            // DET-421: stay quiet for the first half of the trial, then count down over the last week. The
            // threshold is inclusive - "7 days left" is the first day that prompts.
            //
            // A null count means the trial endpoint could not be read. Silence is the safer reading: the
            // alternative is prompting someone on day 1 with copy that cannot say how long they have, which
            // is the behaviour this schedule exists to avoid.
            if (data.IsTrial && !data.IsTrialExpired)
            {
                if (!data.TrialRemainingDays.HasValue || data.TrialRemainingDays.Value > _trialPromptFromDaysRemaining)
                {
                    return null;
                }

                return BuildActiveTrialViewModel(data, accountUri, uriOpener);
            }

            // Trial ended of its own accord. Deliberate cancellations never reach here - the workflow
            // returns null for those, so no prompt is built at all.
            if (data.IsTrial && data.IsTrialExpired)
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

        /// <summary>
        /// The active-trial prompt, shown over the final week of the trial. Only reached with a known day
        /// count, so the copy can name it.
        /// </summary>
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

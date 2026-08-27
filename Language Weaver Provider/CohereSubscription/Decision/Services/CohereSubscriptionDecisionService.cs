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
        /// The active-trial prompt is withheld until /weaver/details/ carries a trial start or end date.
        /// Without one the pop-up cannot honour DET-421's day-based schedule or name the days remaining, so it
        /// would fire at any point in the 14 days saying nothing actionable. Flip to true once
        /// <see cref="CohereSubscriptionData.TrialRemainingDays"/> is populated.
        /// </summary>
        private const bool ActiveTrialPromptEnabled = false;

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

            // Suspended until the service exposes a trial start or end date. DET-421 asks for silence on days
            // 14-8 and "ends in {X} day(s)" from day 7, but /weaver/details/ returns trialStatus with no dates
            // (see COHERE_CONTEXT.md, "Known gap: trial days"), so the only implementable behaviour was to
            // prompt at any point in the 14 days with the {X} omitted - too eager, and unable to say the one
            // thing that makes it useful. Better to say nothing until a date is available.
            //
            // To restore: set ActiveTrialPromptEnabled to true once TrialRemainingDays is populated.
            if (data.IsTrial && !data.IsTrialExpired)
            {
                return ActiveTrialPromptEnabled
                    ? BuildActiveTrialViewModel(data, accountUri, uriOpener)
                    : null;
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
        /// The active-trial prompt, preserved verbatim but not currently reachable: see
        /// <see cref="ActiveTrialPromptEnabled"/>. The copy omits the days remaining, which is precisely why it
        /// is withheld - DET-421 wants "ends in {X} day(s)", and no date is available to compute {X}.
        /// </summary>
        private static SubscriptionViewModel BuildActiveTrialViewModel(CohereSubscriptionData data, string accountUri, IUriOpener uriOpener)
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
    }
}

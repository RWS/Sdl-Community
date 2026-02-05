using LanguageWeaverProvider.Model.Interface;

namespace LanguageWeaverProvider.SubscriptionJourney.Model
{
    public class SubscriptionOptions : ISubscriptionOptions
    {
        public string Title { get; }
        
        public string Description { get; }
        
        public bool IsDoNotShowAgainVisible { get; }

        public string TrialUri { get; }

        public string BuyUri { get; }

        public bool DoNotShowThisAgain { get; }

        public bool ShowPrimary { get; }

        public string PrimaryContent { get; }

        public bool ShowSecondary { get; }

        public string SecondaryContent { get; }

        public string CancelContent { get; }
    }
}

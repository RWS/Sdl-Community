using LanguageWeaverProvider.Model.Interface;

namespace LanguageWeaverProvider.SubscriptionJourney.Model
{
    public class SubscriptionOptions : ISubscriptionOptions
    {
        public string Title { get; set; }
        
        public string Description { get; set; }

        public bool IsDoNotShowAgainVisible { get; set; }

        public string TrialUri { get; set; }

        public string BuyUri { get; set; }

        public bool DoNotShowThisAgain { get; set; }

        public bool ShowPrimary { get; set; }

        public string PrimaryContent { get; set; }

        public bool ShowSecondary { get; set; }

        public string SecondaryContent { get; set; }

        public string CancelContent { get; set; }
    }
}

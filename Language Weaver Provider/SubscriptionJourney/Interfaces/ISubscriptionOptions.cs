using System.Security.Principal;

namespace LanguageWeaverProvider.Model.Interface
{
    public interface ISubscriptionOptions
    {
        public string Title { get; }
        public string Description { get; }
        public bool IsDoNotShowAgainVisible { get; }
        public bool DoNotShowThisAgain { get; }
        public string PrimaryUri { get; }
        public string SecondaryUri { get; }
        public bool ShowPrimary { get; }
        public string PrimaryContent { get; }
        public bool ShowSecondary { get; }
        public string SecondaryContent { get; }
        public string CancelContent { get; }


    }
}

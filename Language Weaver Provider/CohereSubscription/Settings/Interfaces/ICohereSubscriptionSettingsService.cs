namespace LanguageWeaverProvider.CohereSubscription.Settings.Interfaces
{
    public interface ICohereSubscriptionSettingsService
    {
        bool GetDoNotShowAgain();
        void SetDoNotShowAgain(bool value);
    }
}

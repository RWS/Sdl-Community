namespace LanguageWeaverProvider.CohereSubscription.Settings.Model
{
    public class DeveloperSettings
    {
        public string Environment { get; set; }

        public int? TrialPromptFromDaysRemaining { get; set; }

        // Replaces the trialStatus on the real Account Portal response: NOT_STARTED, IN_PROGRESS, CANCELLED, ENDED.
        public string TrialStatusOverride { get; set; }
    }
}

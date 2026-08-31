namespace LanguageWeaverProvider.CohereSubscription.Settings.Model
{
    /// <summary>
    /// Test-time overrides for behaviour that is otherwise fixed at build time. The file this maps to is
    /// written with production defaults the first time the plug-in runs, so an untouched installation
    /// behaves exactly as if the file did not exist.
    /// <para>
    /// This exists to make the Cohere entitlement journey testable by hand. Verifying the trial countdown
    /// otherwise meant editing a constant and rebuilding, and pointing at UAT meant editing
    /// <see cref="Model.CloudEnvironment.Current"/> - which was twice left behind in a commit.
    /// </para>
    /// </summary>
    public class DeveloperSettings
    {
        /// <summary>
        /// The Cloud environment to run against, by name: "Production" or "UAT". An unrecognised or missing
        /// name falls back to production rather than failing, so a typo cannot leave the plug-in pointing
        /// somewhere unusable.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// The active-trial pop-up appears once the trial has this many days left or fewer. The threshold is
        /// inclusive at both ends: the default of 7 prompts from "7 days left" down to and including the
        /// final day.
        /// </summary>
        public int? TrialPromptFromDaysRemaining { get; set; }
    }
}

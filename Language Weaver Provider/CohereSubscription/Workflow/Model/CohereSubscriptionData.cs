namespace LanguageWeaverProvider.CohereSubscription.Workflow.Model
{
    public class CohereSubscriptionData
    {
        public bool IsCohereDetected { get; set; }
        public bool IsPaid { get; set; }
        public bool IsTrial { get; set; }
        public bool IsTrialExpired { get; set; }
        /// <summary>
        /// Whole calendar days until the trial ends, or <c>null</c> when unknown - the trial endpoint was
        /// unreachable, or sent no usable date. Null and 0 are different things: 0 means the trial ends today,
        /// null means we cannot say, and the prompt must not claim a number it does not have.
        /// </summary>
        public int? TrialRemainingDays { get; set; }
        public bool IsAdmin { get; set; }

        /// <summary>
        /// The Account Portal identifier of the account, or <c>null</c> when it has no Account Portal record.
        /// Present only on the Account Portal path; the entitlement itself never depends on it, but it is what
        /// lets the prompt link a provisioned account straight to its own portal page.
        /// </summary>
        public string BusinessAccountId { get; set; }

        /// <summary>
        /// The Account Portal identifier of the account's subscription. Can be <c>null</c> even when
        /// <see cref="BusinessAccountId"/> is present. Not used for linking, since Account Portal discards a
        /// deep path when the URL is opened from outside the portal; kept because it identifies the
        /// subscription the add-on would be bought against.
        /// </summary>
        public string BusinessSubscriptionId { get; set; }
    }
}

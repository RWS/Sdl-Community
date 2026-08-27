namespace LanguageWeaverProvider.CohereSubscription.Workflow.Model
{
    public class CohereSubscriptionData
    {
        public bool IsCohereDetected { get; set; }
        public bool IsPaid { get; set; }
        public bool IsTrial { get; set; }
        public bool IsTrialExpired { get; set; }
        public int TrialRemainingDays { get; set; }
        public bool IsAdmin { get; set; }

        /// <summary>
        /// The Account Portal identifier of the account, or <c>null</c> when it has no Account Portal record.
        /// Present only on the Account Portal path; the entitlement itself never depends on it, but it is what
        /// lets the prompt link a provisioned account straight to its own portal page.
        /// </summary>
        public string BusinessAccountId { get; set; }
    }
}

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
    }
}

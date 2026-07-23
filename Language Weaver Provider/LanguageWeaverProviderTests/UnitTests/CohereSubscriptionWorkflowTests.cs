using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using LanguageWeaverProvider.CohereSubscription.Workflow.Services;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    public class CohereSubscriptionWorkflowTests
    {
        [Fact]
        public void ActiveProSubscription_IsMappedAsPaid()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                IsProActive = true,
                TrialStatus = "NONE"
            });

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsPaid);
            Assert.False(data.IsTrial);
            Assert.False(data.IsAdmin);
        }

        [Fact]
        public void ActiveTrial_IsMappedWithoutInventingAnExpiry()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                TrialStatus = "TRIAL_ACTIVE"
            });

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsTrial);
            Assert.False(data.IsTrialExpired);
            Assert.False(data.IsPaid);
        }

        [Fact]
        public void ExpiredTrial_IsMappedAsExpired()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                TrialStatus = "TRIAL_EXPIRED"
            });

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsTrial);
            Assert.True(data.IsTrialExpired);
        }
    }
}

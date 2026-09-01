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
                TrialStatus = "NOT_STARTED"
            });

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsPaid);
            Assert.False(data.IsTrial);
        }

        [Fact]
        public void PaidAfterTrialConversion_IsStillPaidDespiteTheCancelledTrialStatus()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                IsProActive = true,
                TrialStatus = "CANCELLED"
            });

            Assert.NotNull(data);
            Assert.True(data.IsPaid);
            Assert.True(data.IsCohereDetected);
        }

        [Fact]
        public void TrialInProgress_IsMappedAsAnActiveTrial()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                TrialStatus = "IN_PROGRESS"
            });

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsTrial);
            Assert.False(data.IsTrialExpired);
            Assert.False(data.IsPaid);
        }

        [Fact]
        public void TrialThatRanItsCourse_IsMappedAsEnded()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                TrialStatus = "ENDED"
            });

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsTrial);
            Assert.True(data.IsTrialExpired);
            Assert.False(data.IsPaid);
        }

        [Fact]
        public void DeliberateCancellation_ShowsNoPromptAtAll()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                TrialStatus = "CANCELLED",
                IsProActive = false // pinned from a live UAT cancellation, where isProActive arrives as JSON null
            });

            Assert.Null(data);
        }

        [Fact]
        public void NoTrialAndNoPro_IsMappedAsNotDetected()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                IsProActive = false,
                TrialStatus = "NOT_STARTED"
            });

            Assert.False(data.IsCohereDetected);
            Assert.False(data.IsPaid);
            Assert.False(data.IsTrial);
        }

        [Fact]
        public void NeverTrialed_WithoutProPairs_IsOfferedTheTrial()
        {
            var data = CohereSubscriptionWorkflow.MapEntitlement(hasProLanguagePairs: false);

            Assert.False(data.IsCohereDetected);
            Assert.False(data.IsPaid);
            Assert.False(data.IsTrial);
            Assert.False(data.IsTrialExpired);
        }

        [Fact]
        public void NeverTrialed_WithProPairs_IsPaid()
        {
            var data = CohereSubscriptionWorkflow.MapEntitlement(hasProLanguagePairs: true);

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsPaid);
            Assert.False(data.IsTrial);
        }

        [Fact]
        public void MissingTrialStatus_IsMappedAsNotDetected()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails());

            Assert.False(data.IsCohereDetected);
            Assert.False(data.IsTrial);
            Assert.False(data.IsTrialExpired);
        }
    }
}

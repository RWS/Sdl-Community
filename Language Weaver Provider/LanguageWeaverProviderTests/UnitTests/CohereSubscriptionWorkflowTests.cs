using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using LanguageWeaverProvider.CohereSubscription.Workflow.Services;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Pins the mapping from the account-portal details response onto the pop-up decision inputs.
    /// The <c>trialStatus</c> values used here are the ones the details endpoint actually returns
    /// (NOT_STARTED / IN_PROGRESS / CANCELLED), not invented labels.
    /// </summary>
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
            Assert.False(data.IsAdmin);
        }

        [Fact]
        public void PaidAfterTrialConversion_IsStillPaid()
        {
            // Converting to paid cancels the trial, so this is the ordinary state of a paying customer.
            // Reading it as an expired trial would prompt someone who has already bought the add-on.
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                IsProActive = true,
                TrialStatus = "CANCELLED"
            });

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
        public void CancelledTrialWithoutPro_IsMappedAsExpired()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                TrialStatus = "CANCELLED"
            });

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsTrial);
            Assert.True(data.IsTrialExpired);
            Assert.False(data.IsPaid);
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
        public void MissingTrialStatus_IsMappedAsNotDetected()
        {
            // The endpoint returns NOT_STARTED defaults when no details exist; a null status must not be
            // read as a trial in any state.
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails());

            Assert.False(data.IsCohereDetected);
            Assert.False(data.IsTrial);
            Assert.False(data.IsTrialExpired);
        }
    }
}
